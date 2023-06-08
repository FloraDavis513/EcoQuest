<template>
    <div id="help">
      <div>
          <strong>Правила</strong>
      </div>
      <div class="scroll" style="width:80%;margin-top:1vmax;white-space:pre-wrap;text-align:left;">
        <div>Перед Вами игра-викторина, в которой Вам необходимо отвечать на вопросы, связанные с экосистемой компании.</div>
        <div style="margin-top:15px;">В викторине существует три режима:</div>
        <div>1) тренировочный;</div>
        <div>2) случайный;</div>
        <div>3) соревновательный.</div>
        <div style="margin-top:15px;">В тренировочном режима Вам необходимо выбрать те продукты, которые должны быть включены в тренировку. В саму викторину попадут все вопросы, которые доступны в данный момент по выбранному продукту или продуктам. Кроме того, если будут выявлены продукты, которые вызывают наибольшие трудности, система автоматически предложит выбрать для тренировки именно их.</div>
        <div style="margin-top:15px;">В случайном режиме Вам будут предложены 15 случайных вопросов по всем продуктам.</div>
        <div style="margin-top:15px;">Для участия в викторине в соревновательном режиме Вам необходимо запросить пароль для входа у своего администратора. Вопросы будут подобраны в соответствии с целью соревнования.</div>
        <div style="margin-top:15px;">В ходе викторины Вам будут доступны подсказки:</div>
        <div>1) <strong>50 на 50</strong> - убирает два неверных ответа. Может быть использована только для вопросов с выбором варианта ответа.</div>
        <div id="group">
          <div>Внешний вид подсказки:</div>
          <div class="help_img" title="Убрать два неверных ответа">50:50</div>
        </div>
        <div style="margin-top:3vmax;">2) <strong>Право на ошибку</strong> - позволяет однократно допустить ошибку при вводе/выборе ответа. Доступна для любых типов вопросов.</div>
        <div id="group">
          <div>Внешний вид подсказки:</div>
          <div class="help_img" title="Право на ошибку" style="font-size:1.5vmax;">2 шанс</div>
        </div>
      </div>
      <div class="button" @click="to_mode_switcher">
        Назад
      </div>
    </div>
</template>

<script>
export default {
  name: 'QuizRules',
  data(){
    return {
    }
  },
  methods:{
    to_mode_switcher: function () {
        this.$emit('switch-mode');
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
    height: 90%;
}

#group{
  margin-top: 1vmax;
  display: flex;
  flex-direction: row;
  align-items: center;
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

.help_img{
    width: 5vmax;
    height: 2vmax;
    border: solid 0.25vmax black;
    border-radius: 1vmax;
    padding: 1vmax 0.5vmax 1vmax 0.5vmax;
    text-align: center;
    margin-left: 1vmax;
}

.help_img:hover{
    border: solid 0.25vmax red;
}

.scroll{
    height: 80%;
    overflow: auto;
    /* -ms-overflow-style: none;
    scrollbar-width: none; */
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}
</style>