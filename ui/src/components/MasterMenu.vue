<template>
  <div id="menu" class="menu">
    <input id="template_name" type="text" placeholder="Шаблон комнаты" class="themes_template" :value="template.name" @blur="save_tempalte_name">
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
        <div class="master_menu_button" id="save" @click="save_template">
            Сохранить
        </div>
        <div class="master_menu_button" id="delete" @click="check_delete">
            Удалить
        </div>
        <div class="master_menu_button" @click="share_template" >
            Поделиться
        </div>
    </div>
  </div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'MasterMenu',
  props: ['template', 'count', 'questions'],
  data () {
    return {
    }
  },
  methods: {
    share_template: function () {
      this.$emit('share-template')
    },
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
    back_to_templates: function () {
      this.$emit('back-to-templates')
    },
    save_tempalte_name: function () {
      this.$emit('save-tmpl-name', document.getElementById('template_name').value)
    },
    save_template: async function () {
      this.template.products.forEach(item => {
        if(item.round == 2 && item.second_round_repeating != null)
        {
            let result = '';
            item.second_round_repeating.forEach(field => result = result + String(field));
            item.numOfRepeating = Number(result);
        }
      });
      await fetch(SERVER_PATH + "/gameBoard/update", {
        method: "POST",
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(this.template)
      });
    },
  },
  mounted: function () {
  this.$nextTick(function () {
    var question = document.getElementById('select_question')
    // eslint-disable-next-line camelcase
    var themes = document.getElementById('select_themes')
    themes.style.opacity = 1
    document.getElementById('select_question').value = question.style.opacity = 0.5
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
    border-radius: 1vw;
    padding-top: 4%;
    padding-bottom: 4%;
    margin-left: 10%;
    width: 80%;
    color: white;
    background-color: rgb(38, 146, 17);
    font-size: 1.1vw;
    font-weight: bold;
}

.master_menu_button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

#first_group_button{
  margin-top: 10%;
}

#second_group_button{
  margin-top: 60%;
}
</style>
