<template>
    <div v-if="result" id="results">
        <div>Результаты</div>
        <div>{{"Верных ответов: " + result.correct_answer + " из " + result.total_question }}</div>
        <div>{{"Общее время: " + beautify_timer(result.total_time)}}</div>
        <div class="button" @click="login_player">Меню</div>
    </div>
</template>

<script>
import {SERVER_PATH} from "@/common_const";

export default {
  name: 'QuizResults',
  components: {
  }, 
  data(){
    return {
        result: null
    }
  },
  methods:{
    beautify_timer: function(elapsed_seconds) {
        const elapsed_minutes = Math.floor(elapsed_seconds / 60);
        const remain_seconds = elapsed_seconds - elapsed_minutes * 60;
        return ((elapsed_minutes < 10) ? ("0" + String(elapsed_minutes)) : elapsed_minutes) + ":" + ((remain_seconds < 10) ? ("0" + String(remain_seconds)) : remain_seconds);
    },
    login_player: function () {
        this.$emit('login-player');
    },
  },
  beforeCreate: async function() {
    await fetch(SERVER_PATH + '/quiz/result/' + JSON.parse(localStorage.getItem('user')).userId, {
                method: 'GET',
                headers: {'Content-Type': 'application/json'},
              }).then(res => res.json()).then(data => this.result = data);
  },
  
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#results{
    margin-top: 2%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
}

.button{
    width: 30vmax;
    margin-top: 1%;
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