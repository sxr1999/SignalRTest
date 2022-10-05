<template>
  <input type="text" v-model="state.userMsg" v-on:keypress="txtMsgOnKeypress"/>
  
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
    const state = reactive({userMsg:"",messages:[],userName:"",passWord:""});

    const login =async function(){
      
      const payload ={userName:state.userName,passWord:state.passWord};
        axios.post('https://localhost:7273/api/Demo/Login',payload)
        .then(res=>{
          alert(res.data);
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
    onMounted(async function(){
      connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7273/MyHub",{skipNegotiation:true,transport:signalR.HttpTransportType.WebSockets})
      .withAutomaticReconnect().build();
      await connection.start();
      connection.on('PublicMsgReceived',rcvMsg=>{
        state.messages.push(rcvMsg);
      });
    });
    return {state,txtMsgOnKeypress,login};
   }
}

</script>



<style scoped>

.read-the-docs {
  color: #888;
}
</style>
