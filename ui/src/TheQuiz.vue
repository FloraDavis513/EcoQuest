<template>
    <GamerHeader @logout="log_out" @change-pass="change_pass = true" />
    <QuizResults v-if="results" @login-player="login_player"/>
    <QuizField v-else :quiz_products="quiz_products" :mode="mode" @end-quiz="end_quiz" />
</template>

<script>
import GamerHeader from './components/GamerHeader.vue'
import QuizField from './components/QuizField.vue'
import QuizResults from './components/QuizResults.vue'

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
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>