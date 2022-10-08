<template>
  <div>
    <h3>公屏发送</h3>
    内容:<input type="text" v-model="state.userMsg" v-on:keypress="txtMsgOnKeypress"/>
  </div>
  <h4>=====================================</h4>
  <div>
    <h3>私聊</h3>
    目标用户名: <input type="text" v-model="state.toUserName"/>
    内容:<input type="text" v-model="state.PrivateuserMsg" v-on:keypress="txPrivateMsgOnKeypress"/>
  </div>
  <h4>=====================================</h4>
  <h3>登录</h3>
  <div>
        <input type="text" v-model="state.userName"/>
        <input type="text" v-model="state.passWord" />
        <button v-on:click="login">登录</button>
    </div>

    <div>
      <ul>
          <li v-for="(msg,index) in state.messages" :key="index">{{msg}}</li>
      </ul>
    </div>

</template>

<script >
import { ref,onMounted, reactive } from 'vue';
import * as signalR from '@microsoft/signalr';
import axios from  'axios';
let connection;

export default {
   setup(){
    const state = reactive({userMsg:"",messages:[],userName:"",passWord:"",toUserName:"",PrivateuserMsg:""});

    const login =async function(){
      
      const payload ={userName:state.userName,passWord:state.passWord};
        axios.post('https://localhost:7273/api/Demo/Login',payload)
        .then(async res=>{
          const token = res.data;

          var options = {skipNegotiation:true,transport:signalR.HttpTransportType.WebSockets};
          options.accessTokenFactory=()=>token;
          connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7273/MyHub",options)
          .withAutomaticReconnect().build();
          await connection.start();


          connection.on('PublicMsgReceived',rcvMsg=>{
            state.messages.push(rcvMsg);
          });

          
          connection.on('PrivateMegReceived',(fromUserName,msg)=>{
            state.messages.push("用户"+fromUserName+"对你私聊说："+msg);
          });
        })
        .catch(err=>{
          console.log(err)
        })
    };

    const txtMsgOnKeypress = async function(e){
      if(e.keyCode != 13)
      return;
      await connection.invoke("SendPublicMessage",state.userMsg);
      state.userMsg="";
    };

    const txPrivateMsgOnKeypress = async function(e){
      if(e.keyCode != 13)
      return;
      await connection.invoke("SendPrivateMsg",state.toUserName,state.PrivateuserMsg);
      state.PrivateuserMsg="";
    };



    onMounted(async function(){

      
    });
    return {state,txtMsgOnKeypress,login,txPrivateMsgOnKeypress};
   }
}

</script>



<style scoped>

.read-the-docs {
  color: #888;
}
</style>
