<template>
    <div id="help" @click="to_mode_switcher">
      <div>
          Правила
      </div>
      <div style="width:80%;margin-top:1vmax;white-space:pre-wrap;text-align:left;">
        <div>В викторине присутствуют два вида подсказок:</div>
        <div>1) <em>{{"<здесь будет иконка>"}}</em> <strong>50 на 50</strong> - убирает два неверных ответа. Может быть использована только для вопросов с выбором варианта ответа.</div>
        <div>2) <em>{{"<здесь будет иконка>"}}</em> <strong>Право на ошибку</strong> - позволяет однократно допустить ошибку при вводе/выборе ответа. Доступна для любых типов вопросов.</div>
      </div>
      <div class="button">
        Назад
      </div>
    </div>
</template>

<script>
export default {
  name: 'QuizRules',
  data(){
    return {
        mode: 'type'
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