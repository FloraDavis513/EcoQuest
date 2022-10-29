<template>
  <div id="menu" class="menu">
    <input id="template_name" type="text" placeholder="Шаблон комнаты" class="themes_template" @blur="save_tempalte_name">
    <div id="first_group_button">
        <div class="master_menu_button" @click="select_question" id="select_question">
            Выбор вопросов
        </div>
        <div class="master_menu_button" @click="select_themes" id="select_themes">
            Выбор тем
        </div>
        <div class="master_menu_button" @click="back_to_templates" >
            К другим шаблонам
        </div>
    </div>
    <div id="second_group_button" >
        <div class="master_menu_button" id="save" @mouseover="get_questions" @click="save_template">
            Сохранить
        </div>
        <div class="master_menu_button" id="delete" @click="check_delete">
            Удалить
        </div>
        <div class="master_menu_button" >
            Поделиться
        </div>
        <div class="master_menu_button" id="start" @click="create_game">
            Запустить игру
        </div>
    </div>
</div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'MasterMenu',
  props: ['template_name', 'count', 'questions', 'tmpl_id'],
  data () {
    return {
    }
  },
  methods: {
    select_question: function () {
      var question = document.getElementById('select_question')
      // eslint-disable-next-line camelcase
      var themes = document.getElementById('select_themes')
      themes.style.opacity = 1
      question.style.opacity = 0.5
      this.$emit('select-question')
    },
    select_themes: function () {
      var question = document.getElementById('select_question')
      // eslint-disable-next-line camelcase
      var themes = document.getElementById('select_themes')
      themes.style.opacity = 0.5
      question.style.opacity = 1
      this.$emit('select-themes')
    },
    check_delete: function () {
      this.$emit('check-delete')
    },
    create_game: function () {
      this.$emit('create-game')
    },
    back_to_templates: function () {
      this.$emit('back-to-templates')
    },
    save_tempalte_name: function () {
      this.$emit('save-tmpl-name', document.getElementById('template_name').value)
    },
    get_questions: function () {
      this.$emit('get-questions')
    },
    sleep: function (milliseconds) {
            var start = new Date().getTime();
            for (var i = 0; i < 1e7; i++) {
                if ((new Date().getTime() - start) > milliseconds){
                break;
                }
            }
        },
    save_template: function () {
      if(!this.tmpl_id)
      {
        fetch(SERVER_PATH + "/board/create", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({name: this.template_name, productWithQuestionRqs: this.questions, numFields: this.count ? this.count : 16})
                });
        this.sleep(300);
        this.$emit('back-to-templates');
      }
      else
      {
        fetch(SERVER_PATH + "/board/update", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({id:this.tmpl_id, name: this.template_name, productWithQuestionRqs: this.questions, numFields: this.count ? this.count : 16})
                });
        this.sleep(300);
        this.$emit('back-to-templates');
      }
      
    },
  },
  mounted: function () {
  this.$nextTick(function () {
    // Код, который будет запущен только после
    // отображения всех представлений
    var question = document.getElementById('select_question')
    // eslint-disable-next-line camelcase
    var themes = document.getElementById('select_themes')
    themes.style.opacity = 1
    document.getElementById('select_question').value = question.style.opacity = 0.5
    document.getElementById('template_name').value = this.template_name
    
  })
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.themes_template{
  width: 70%;
  height: 20%;
  text-align: center;
  font-size: 1.3vw;
  border-radius: 10px;
}
.menu{
    float: left;
    width: 20%;
    text-align: center;
    height: 88%;
    padding-top: 1%;
    padding-bottom: 1%;
    border-right: 2px solid silver;
    margin-right: 1%;
}

.avatar{
    margin-left: 15%;
    width: 70%;
    color: #ffffff;
    padding-top: 15%;
    padding-bottom: 15%;
    border-radius: 20px;
    border: solid black 2px;
    font-size: 180%;
}

.master_menu_button{
    margin-top: 4%;
    border-radius: 20px;
    padding-top: 4%;
    padding-bottom: 4%;
    margin-left: 10%;
    width: 80%;
    color: white;
    background-color: rgb(38, 146, 17);
    font-size: 1vw;
    font-weight: bold;
}

.master_menu_button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

#first_group_button{
  margin-top: 10%;
}

#second_group_button{
  margin-top: 40%;
}
</style>
