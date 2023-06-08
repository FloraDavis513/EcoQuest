<template>
    <GamerHeader @logout="log_out" @change-pass="change_pass = true" @to-gamer-stat="to_gamer_stat" />
    <ModeSwitcher v-if="menu == 'menu'" @pick-themes="pick_themes" @show-help="show_help" @start-quiz="start_quiz" @start-challenge-quiz="start_quiz" @show-stat="show_stat" @show-rate="show_rate" />
    <ThemesPicker v-if="menu == 'themes'" :mode="mode" @switch-mode="switch_mode" @start-quiz="start_quiz" />
    <QuizRules v-if="menu == 'help'" @switch-mode="switch_mode" />
    <PlayerStat v-if="menu == 'stat'" @switch-mode="switch_mode" />
    <PlayerRate v-if="menu == 'rate'" @switch-mode="switch_mode" />
</template>

<script>
import GamerHeader from './components/GamerHeader.vue'
import ModeSwitcher from './components/ModeSwitcher.vue'
import ThemesPicker from './components/ThemesPicker.vue'
import QuizRules from './components/QuizRules.vue'
import PlayerStat from './components/PlayerStat.vue'
import PlayerRate from './components/PlayerRate.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'GamerMenu',
  components: {
    GamerHeader,
    ModeSwitcher,
    ThemesPicker,
    QuizRules,
    PlayerStat,
    PlayerRate
  }, 
  data(){
    return {
      menu: 'menu',
      mode: 'train'
    }
  },
  methods:{
    pick_themes: function (mode) {
        this.menu = 'themes';
        this.mode = mode;
    },
    switch_mode: function () {
        this.menu = 'menu';
    },
    show_help: function () {
        this.menu = 'help';
    },
    show_stat: function () {
        this.menu = 'stat';
    },
    show_rate: function () {
        this.menu = 'rate';
    },
    start_quiz: function (products, mode) {
        this.$emit('start-quiz', products, mode);
    },
    log_out: function () {
        this.$emit('logout');
    },
    to_gamer_stat: function () {
        this.$emit('to-gamer-stat');
    },
  },
  beforeCreate: async function () {
      if(!localStorage.getItem('user'))
        this.log_out();
      await fetch(SERVER_PATH + "/gamer", {
              method: "GET",
              headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
      }).then(res => {
          if(res.status === 401 || res.status === 403)
              this.log_out();
      });
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>