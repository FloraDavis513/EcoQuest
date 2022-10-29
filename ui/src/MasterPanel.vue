<template>
    <MasterHeader @logout="log_out" />
    <GameStatus />
</template>

<script>
import MasterHeader from './components/MasterHeader.vue'
import GameStatus from './components/GameStatus.vue'

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
  }
  
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>