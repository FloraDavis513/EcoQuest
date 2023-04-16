<template>
    <GamerHeader @logout="log_out" @change-pass="change_pass = true" @to-gamer-stat="to_gamer_stat" />
    <NavigationGamerStat />
    <GamerStatMenu />
</template>

<script>
import GamerHeader from './components/GamerHeader.vue'
import NavigationGamerStat from './components/NavigationGamerStat.vue'
import GamerStatMenu from './components/GamerStatMenu.vue'

export default {
  name: 'GamerStat',
  components: {
    GamerHeader,
    NavigationGamerStat,
    GamerStatMenu
  }, 
  data(){
    return {
    }
  },
  methods:{
    log_out: function () {
        this.$emit('logout');
    },
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#help{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
    margin-top: 1vmax;
}

.button{
    width: 30vmax;
    margin-top: 1vmax;
    background-color: green;
    color: #ffffff;
    font-weight: bold;
    border-radius: 1.5vmax;
    padding-top: 1vmax;
    padding-bottom: 1vmax;
    text-align: center;
}

.button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>