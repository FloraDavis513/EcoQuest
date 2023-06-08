<template>
    <div id="menu">
        <div v-if="mode == 'type'" id="menu_header">Меню</div>
        <div v-if="mode == 'type'" class="button" @click="mode = 'quiz'">Викторина</div>
        <div v-if="mode == 'type'" class="button" @click="mode = 'quest'">Командная игра</div>
        <div v-if="mode == 'type'" class="button" @click="show_stat">Статистика</div>
        <div v-if="mode == 'type'" class="button" @click="show_rate">Рейтинг</div>
        <!-- <div v-if="mode == 'type'" class="button">Достижения</div> -->
        <div v-if="mode == 'quiz'" id="menu_header">Викторина</div>
        <div v-if="mode == 'quiz'" class="button" @click="start_quiz">Случайные режим</div>
        <div v-if="mode == 'quiz'" class="button" @click="pick_themes('train')">Тренировочный режим</div>
        <!-- <div v-if="mode == 'quiz'" class="button" @click="pick_themes('challenge')">Соревновательный режим</div> -->
        <div v-if="mode == 'quiz'" class="button" @click="mode = 'challenge'">Соревновательный режим</div>
        <div v-if="mode == 'quiz'" class="button" @click="show_help">Правила</div>
        <div v-if="mode == 'quiz'" class="button" @click="mode = 'type'">Назад</div>
        <div v-if="mode == 'quest'" id="menu_header">Командная игра</div>
        <input v-if="mode == 'quest'" type="text" class="form-control" id="middlename" placeholder="Введите id игры">
        <div v-if="mode == 'quest'" class="button">Войти</div>
        <div v-if="mode == 'quest'" class="button" @click="mode = 'type'">Назад</div>
        <div v-if="mode == 'challenge'" id="menu_header">Соревновательный режим</div>
        <input v-if="mode == 'challenge'" type="text" class="form-control" id="password" placeholder="Введите пароль">
        <div v-if="mode == 'challenge'" class="button" @click="start_challenge_quiz">Начать</div>
        <div v-if="mode == 'challenge'" class="button" @click="mode = 'type'">Назад</div>
    </div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'ModeSwitcher',
  data(){
    return {
        mode: 'type'
    }
  },
  methods:{
    pick_themes: function (mode) {
        this.$emit('pick-themes', mode);
    },
    start_quiz: function () {
        this.$emit('start-quiz', []);
    },
    start_challenge_quiz: function () {
        fetch(SERVER_PATH + "/challenge/check", {
        method: "POST",
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({password: document.getElementById("password").value})
        }).then(res => {
              if(res.status !== 200)
                  res.text().then(text => alert(text));
              else
                  this.$emit('start-quiz', [], 'challenge');
          });
    },
    show_help: function () {
        this.$emit('show-help');
    },
    show_stat: function () {
        this.$emit('show-stat');
    },
    show_rate: function () {
        this.$emit('show-rate');
    },
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#menu{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
}

#menu_header{
    margin-top: 1%;
    margin-bottom: 1%;
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

.form-control{
    width: 30vmax;
    font-size: 1.9vmax;
    border-bottom: 0.1vw solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    margin-top: 1%;
}
</style>