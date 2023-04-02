<template>
    <MasterHeader @logout="log_out" />
    <GameStatus />
</template>

<script>
import MasterHeader from './components/MasterHeader.vue'
import GameStatus from './components/GameStatus.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'RegistrationForm',
  components: {
    MasterHeader,
    GameStatus
  }, 
  data(){
    return {
      
    }
  },
  methods:{
    log_out: function(){
        this.$emit('logout');
    },
  },
  beforeCreate: async function () {
        await fetch(SERVER_PATH + "/status", {
                method: "GET",
                headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
        }).then(res => {
            if(res.status === 401)
                this.log_out();
        });
    }
  
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>