<template>
    <GamerHeader @logout="log_out" @change-pass="change_pass = true" />
    <QuizResults v-if="results" @login-player="login_player"/>
    <QuizField v-else :quiz_products="quiz_products" :mode="mode" @end-quiz="end_quiz" />
</template>

<script>
import GamerHeader from './components/GamerHeader.vue'
import QuizField from './components/QuizField.vue'
import QuizResults from './components/QuizResults.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'TheQuiz',
  components: {
    GamerHeader,
    QuizField,
    QuizResults
  }, 
  props: ['quiz_products', 'mode'],
  data(){
    return {
      results: false
    }
  },
  methods:{
    end_quiz: function () {
        this.results = true;
    },
    login_player: function () {
        this.$emit('login-player');
    },
    log_out: function () {
        this.$emit('log_out');
    },
  },
  beforeCreate: async function () {
      if(!localStorage.getItem('user'))
        this.log_out();
      await fetch(SERVER_PATH + "/quiz", {
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