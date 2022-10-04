<script >
import { ref,onMounted, reactive } from 'vue';
import * as signalR from '@microsoft/signalr';
let connection;

export default {
   setup(){
    const state = reactive({userMsg:"",messages:[]});
    const txtMsgOnKeypress = async function(e){
      if(e.keyCode != 13)
      return;
      await connection.invoke("SendPublicMessage",state.userMsg);
      state.userMsg="";
    };
    onMounted(async function(){
      connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7273/MyHub")
      .withAutomaticReconnect().build();
      await connection.start();
      connection.on('PublicMsgReceived',rcvMsg=>{
        state.messages.push(rcvMsg);
      });
    });
    return {state,txtMsgOnKeypress};
   }
}

</script>

<template>
  <input type="text" v-model="state.userMsg" v-on:keypress="txtMsgOnKeypress"/>
  <div>
    <ul>
      <li v-for="(msg,index) in state.messages" :key="index">{{msg}}</li>
    </ul>
  </div>
</template>

<style scoped>

.read-the-docs {
  color: #888;
}
</style>
