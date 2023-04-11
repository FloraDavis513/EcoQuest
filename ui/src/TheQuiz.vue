<template>
    <GamerHeader @logout="log_out" @change-pass="change_pass = true" />
    <QuizResults v-if="results" />
    <QuizField v-else :quiz_products="quiz_products" @end-quiz="end_quiz" />
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
  props: ['quiz_products'],
  data(){
    return {
      results: false
    }
  },
  methods:{
    end_quiz: function () {
        this.results = true;
    },
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>