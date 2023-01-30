<template>
<div id="question_preview" v-show="preview_question && visible == 1" @click="preview_question = false">
    <div class="scroll"><pre>{{ preview_text }}</pre></div>
</div>
<div v-if="visible==5" id="game_list">
  <div class="scroll" style="width:78%;margin-top:5%;height:100%;">
      <img @click="to_games" src="@/assets/go_back.png" style="float:left;width:5%;height:10%;" alt="">
      <div style="width:100%;height:10%;margin-left:10%;margin-top:2%;">
          <div style="width:20%;float:left;height:40%;margin-right:50%;">
            <div style="font-size:1.2vw;width:50%;height:35%;">{{'Id: ' + current_game.gameId}}</div>
            <!-- <div type="text" style="font-size:1.2vw;width:60%;" id="game_id" :value="current_game.gameId"/> -->
          </div>
          <div style="width:33%;float:left;height:100%;margin-top:1.5%;">
            <div style="font-size:1.2vw;width:70%;height:50%;">Название</div>
            <input type="text" style="font-size:1.2vw;width:100%;" id="game_name" @blur="save_game_data" :value="current_game.name">
          </div>
          <div style="width:33.5%;float:left;height:100%;margin-right:50%;margin-top:1%;">
            <div style="font-size:1.2vw;width:100%;height:50%;" @blur="save_game_data">Дата</div>
            <Datepicker v-model="date" :format-locale="ru" locale="ru" select-text="Подтвердить" cancel-text="Закрыть"/>
          </div>
      </div>
      <FirstRoundField style="top:12%;left:52.5%;transform: scale(0.85);" @new-message="update_message" :logos="[]" :current_template="template" :game_settings="current_game" />
      <div class="game_buttons" style="top:64.75%;left:22.5%;" id="save_game_button" @click="reset_game">Сброс</div>
      <div class="game_buttons" style="top:84.75%;left:22.5%;" id="save_game_button" @click="save_game">Сохранить</div>
      <div class="game_buttons" style="top:84.75%;left:39%;" id="save_game_button" @click="delete_game">Удалить</div>
      <div class="game_buttons" style="top:92%;left:22.5%;" id="shuffle_button" @click="custom_shuffle(current_game.state.field_config)">Перемешать</div>
      <div class="game_buttons" style="top:92%;left:39%;" id="start_button" @click="create_game">Начать</div>
  </div>
</div>
<div v-if="visible==4" id="game_list">
  <div class="scroll" style="width:78%;margin-top:5%;height:100%;">
      <div class="grid" id="grid">
          <div class="grid_element" v-for="(option, index) in game_list" :key="index" :index="index" @click="select_game(index)">{{ option.name }}</div>
          <div class="grid_element plus" @click="add_game">+</div>
      </div>
    </div>
</div>
<div v-if="visible==3" id="group_delete">
    <div id="delete_product_warning">
          Вы действительно хотите удалить этот шаблон?
      </div>
      <div class="delete_window_group_button">
          <div @click="delete_tmpl" class="button">Да</div>
          <div @click="cancel_tmpl" class="button">Нет</div>
    </div>
</div>
<div v-if="visible==1" class="item_block_scroll">
      <div v-for="(product,index) in get_products_by_round()" :key="index" id="size_themes">
        <div class="themes_themes" :style="product.colour" >
            <div class="text_themes" >
              {{product.name}}
            </div>
            <button class="button_themes" @click="expand_product(product, $event)"><img class="polygon" src="@/assets/Polygon_1.png"></button>
        </div>
        <div class="quest_themes" v-show="product.visible_question">
            <div class="last_redaction" style="font-weight: bold;text-decoration: underline;text-align:center;margin-left: 4%;">
              Изменен
            </div>
            <div class="name_quest2" style="font-weight: bold;text-decoration:underline;margin-left: 6%;">
              Краткое название
            </div>
            <div class="type_quest" style="font-weight: bold;text-decoration: underline;">
              Тип
            </div>
            <div class="check_quest">
                <input :id="'product_check_' + index" class="check_quest_2" type="checkbox" :checked="product.activeQuestions.length != 0" @click="change_all_status(product.productId)">
            </div>
        </div>
        <div v-for="(question,question_index) in product.allQuestions" :key="question_index" class="quest_themes" v-show="product.visible_question" >
            <div class="last_redaction" @click="show_question(question.text, question.answer)">
              {{question.lastEditDate}}
            </div>
            <div class="name_quest2" @click="show_question(question.text, question.answer)">
              {{question.shortText}}
            </div>
            <div class="type_quest" @click="show_question(question.text, question.answer)">
              {{get_readiable_type(question.type)}}
            </div>
            <div class="check_quest">
                <input class="check_quest_2" type="checkbox" :checked="product.activeQuestions.includes(question.questionId)" @click="change_status(product.productId, question.questionId, index)">
            </div>
        </div>
      </div>
</div>
<div v-if="visible == 2 && current_round == 1" class="item_block">
  <div class="count_field">
      <div class="text_fields">
        Число игровых полей
      </div>
        <form style="height:85%">
          <div class="radio_text">
          <input type="radio" id="Choice1" class="radio_fields" checked
          name="Fields_Count" v-on:click="count_field_16">
          <label>16</label>

          <input type="radio" id="Choice2" class="radio_fields"
          name="Fields_Count" v-on:click="count_field_20">
          <label>20</label>

          <input type="radio" id="Choice3" class="radio_fields"
          name="Fields_Count" v-on:click="count_field_24">
          <label >24</label>
          </div>
        </form>
  </div>
  <div class="count_field">
      <div class="text_fields">
        Количество выбранных полей
      </div>
      <label class="fields_number_progressbar">0</label>
      <div class="progressbar">
        <span class="progress_1" id="progress_1"></span>
      </div>
      <label id="total_field" class="fields_number_progressbar">{{calc_sum_field()}} из {{count_field}}</label>
  </div>
  <div class="tex_themes_before">
    Укажите количество повторений темы на игровом поле
  </div>
  <div class="item_block_scroll_2" >
      <div class="block_all_themes_filed" >
        <div v-for="(product,index) in get_products_by_round()" :key="index" id="size_themes_2" >
              <div class="themes_themes_2" :style="product.colour">
                  <div class="text_fields_themes" >
                    {{product.name}}
                  </div>
                  <form style="height:100%;">
                    <div class="radio_text_2">
                      <div v-for="(r_butt,index) in radio_buttons" :key="index" style="height:100%;float:left;width:10%">
                          <input type="radio" class="radio_fields_2" name="Fields_Count" @click="product.numOfRepeating = r_butt, product.current_checked = index, count_field_now()" style="float:left;" :checked="product.numOfRepeating == index">
                          <label style="float:left;margin-top:1%;">{{r_butt}}</label>
                      </div>
                    </div>
                  </form>
              </div>
        </div>
      </div>
  </div>
</div>
<div v-if="visible == 2 && current_round == 2" class="item_block">
  <div :id="'start_sec_round_' + index" class="questions start_sec_round" style="margin-left:8.5%;">Старт</div>
  <div v-for="(q_item, q_index) in second_round_bar" :key="q_index" class="questions" :id="'second_round_question_' + String(index) + String(q_index)">
    <select style="width:90%;height:20%;margin-left:5%;margin-top:40%;font-size:1vw;" :id="'type_selector_' + q_index" @input="select_second_round_product(q_index)">
      <option>Не выбрано</option>
      <option v-for="(product_item, product_index) in get_products_by_round()" :key="product_index">{{product_item.name}}</option>
    </select>
    <img v-if="check_bar(q_index)" src="@/assets/accept.png" style="width:20%;height:20%;margin-left:40%;margin-top:5%;">
  </div>
  <!-- <div :id="'finish_' + index" class="questions"><img src="@/assets/cup.png" id="cup" style="width:90%;height:90%;margin-left:5%;margin-top:5%;"></div> -->
  <div :id="'finish_' + index" class="questions">
    <select style="width:90%;height:20%;margin-left:5%;margin-top:40%;font-size:1vw;" :id="'type_selector_5'" @input="select_second_round_product(5)">
      <option v-for="(product_item, product_index) in get_products_by_round()" :key="product_index">{{product_item.name}}</option>
    </select>
    <img src="@/assets/cup.png" id="cup" style="width:20%;height:20%;margin-left:40%;margin-top:5%;">
  </div>
</div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'
import FirstRoundField from './FirstRoundField.vue'
import Datepicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css'
import { parse } from 'date-fns';


export default {
  name: 'TemplateSettings',
  props: ['visible', 'template', 'current_round'],
  data () {
    return {
      name_template: '',
      count_field: 16,
      count_field_now_pole: 1,
      progress: 0,
      radio_buttons: [
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9
      ],
      current_template: {},
      products: [],
      template_products: [],
      preview_question: false,
      preview_text: '',
      game_list: [],
      current_game: ['', '', ''],
      question: ['','','',''],
      date: null,
      second_round_bar: [false, false, false, false, false],
      second_round_filled: false
    }
  },
  components: {
    FirstRoundField,
    Datepicker
  }, 
  methods: {
    check_bar: function(index) {
      return this.second_round_bar[index];
    },
    fill_second_round_bar: function()
    {
        if(this.second_round_filled)
          return;
        if(!document.getElementById('type_selector_0'))
          return;
        this.second_round_filled = true;
        let second_round = ['Не выбрано', 'Не выбрано', 'Не выбрано', 'Не выбрано', 'Не выбрано', 'Не выбрано'];
        this.template.products.filter((item) => item.round == 2).forEach(item => {
          if(item.numOfRepeating > 0)
            String(item.numOfRepeating).split('').forEach(char => {
              second_round[Number(char) - 1] = item.name;
              if(item.second_round_repeating == null)
                item.second_round_repeating = new Set();
              item.second_round_repeating.add(Number(char));
              });
        });
        for(let i = 0; i < 6; ++i)
        {
          if(i < 5)
            this.second_round_bar[i] = second_round[i] != 'Не выбрано';
          document.getElementById('type_selector_' + i).value = second_round[i];
        }
    },
    select_second_round_product: function(index)
    {
      if(!document.getElementById('type_selector_' + index))
        return;
      let selector = document.getElementById('type_selector_' + index);
      if(index < 5)
      this.second_round_bar[index] = selector.value != 'Не выбрано';

      console.log(this.get_products_by_round());
      this.get_products_by_round().forEach(item => {
        if(item.name == selector.value)
        {
          if(item.second_round_repeating == null)
            item.second_round_repeating = new Set();
          item.second_round_repeating.add(index + 1);
        }
        else
        {
          if(item.second_round_repeating != null)
            item.second_round_repeating.delete(index + 1);
        }
      });

      console.log(this.get_products_by_round());
    },
    get_products_by_round: function()
    {
        return this.template.products.filter((item) => item.round == this.current_round)
    },
    create_game: function () {
      this.$emit('create-game', this.current_game.gameId);
    },
    change_status: function (product_id, question_id, product_index) {
      let global_checked;
      this.template.products.forEach(product => {
        if(product_id == product.productId)
        {
            if(product.activeQuestions.includes(question_id))
              product.activeQuestions = product.activeQuestions.filter(item => item != question_id);
            else
              product.activeQuestions.push(question_id);
            global_checked = product.activeQuestions.length != 0;
        }
      });
      document.getElementById('product_check_' + product_index).checked = global_checked;
    },
    change_all_status: function (product_id) {
      this.template.products.forEach(product => {
        if(product_id == product.productId)
        {
            if(product.activeQuestions.length != 0)
              product.activeQuestions.length = 0;
            else
            {
              product.allQuestions.forEach(question => {
                product.activeQuestions.push(question.questionId);
              })
            }
        }
      });
    },
    to_games: function () { 
      this.$emit('games');
    },
    select_game: async function (index) { 
      await fetch(SERVER_PATH + "/game/get/" + String(this.game_list[index].gameId), {
        method: "GET",
        headers: {'Content-Type': 'application/json'}
      }).then( res => res.json() ).then( data => this.current_game = data );
      this.current_game.state = JSON.parse(this.current_game.state);
      this.date = parse(this.current_game.date, 'M/d/yyyy, hh:mm:ss aa', new Date());
      console.log(this.current_game);
      this.$emit('show-game');
    },
    generate_config: function () {
      let config = [];
      this.get_products_by_round().forEach(item => {
          for( let i = 0; i < item.numOfRepeating; ++i )
          {
            const regex = /logo\d+.\w+/g;
            const found = item.logo ? item.logo.match(regex) : null;
            console.log({name:item.name, colour:item.colour, logo:found ? found[0] : found});
            config.push({name:item.name, colour:item.colour, logo:found ? found[0] : found});
          }
      });
      this.custom_shuffle(config);
      return {field_config: config};
    },
    generate_second_round_config: function () {
      let second_round = [null, null, null, null, null, null];
      this.template.products.filter((item) => item.round == 2).forEach(item => {
        if(item.numOfRepeating > 0)
          String(item.numOfRepeating).split('').forEach(char => {
            second_round[Number(char) - 1] = item;
            });
      });

      return second_round;
    },
    add_game: async function () {
      let current_date = new Date();
      let date_string = current_date.toLocaleString('en-US', { timeZone: 'Asia/Yekaterinburg' });
      let save_date = parse(date_string, 'dd.MM.yyyy, HH:mm:ss', new Date());
      console.log(date_string);
      console.log(save_date);
      let base_config = this.generate_config();
      base_config['numFields'] = this.template.numFields;
      base_config['second_round_config'] = this.generate_second_round_config();
      base_config['questions_round_1'] = this.generate_questions_round_1();
      base_config['Players'] = [];
      base_config['template_id'] = this.template.gameBoardId;
      await fetch(SERVER_PATH + "/game/create", {
        method: "POST",
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({userId:JSON.parse(localStorage.getItem('user')).userId, name:'Новая игра', message:'Приветственное сообщение', date: date_string, state: JSON.stringify(base_config)})
      });
      this.read_games();
    },
    reset_game: async function () {
      if( typeof(this.current_game.state) == 'string' )
            this.current_game.state = JSON.parse(this.current_game.state);
      delete this.current_game.state['current_chip_poses'];
      delete this.current_game.state['current_number'];
      delete this.current_game.state['current_question'];
      delete this.current_game.state['themes'];
      delete this.current_game.state['teams'];
      delete this.current_game.state['helps'];
      delete this.current_game.state['price'];
      delete this.current_game.state['number_round'];
      delete this.current_game.state['scores'];
      delete this.current_game.state['timer'];
      delete this.current_game.state['crit_timer'];
      this.current_game.state['Players'] = [];
      fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({gameId:this.current_game.gameId, state:JSON.stringify(this.current_game.state), currentQuestionId: null}),
            });
    },
    save_game: async function () {
      this.current_game.state.questions_round_1 = this.generate_questions_round_1();
      this.current_game.state.numFields = this.template.numFields;
      this.current_game.state.joined_players = [];
      // this.current_game.state.field_config = this.field_config;
      console.log(this.current_game.state);
      this.current_game.state = JSON.stringify(this.current_game.state);
      this.current_game.date = this.date.toLocaleString('en-US', { timeZone: 'Asia/Yekaterinburg' });
      await fetch(SERVER_PATH + "/game/update", {
        method: "POST",
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(this.current_game)
      });
      this.current_game.state = JSON.parse(this.current_game.state);
    },
    generate_questions_round_1: function () {
      let result = {};

      this.get_products_by_round().forEach(product => {
          let product_with_questions = JSON.parse(JSON.stringify(product));
          product_with_questions.questions = product_with_questions.allQuestions.filter( question => product.activeQuestions.includes(question.questionId) );
          // delete product_with_questions.allQuestions;
          // delete product_with_questions.activeQuestions;
          if(product_with_questions.questions.length > 0)
              result[product_with_questions.name] = product_with_questions.questions;
      });

      return result;
    },
    delete_game: async function () {
      await fetch(SERVER_PATH + "/game/delete/" + String(this.current_game.gameId), {
        method: "DELETE",
        headers: {'Content-Type': 'application/json'},
      });
      this.read_games();
      this.$emit('games');
    },
    save_game_data: function () {
      this.current_game.name = document.getElementById('game_name').value;
      this.current_game.date = document.getElementById('game_date').value;
    },
    update_message: function (new_message) {
      this.current_game.message = new_message;
    },
    read_games: async function () {
      let all_games;
      this.game_list.length = 0;
      await fetch(SERVER_PATH + "/game/get/all/" + String(JSON.parse(localStorage.getItem('user')).userId), {
        method: "GET",
        headers: {'Content-Type': 'application/json'}
      }).then( res => res.json() ).then( data => all_games = data );

      this.game_list = all_games.filter(item => JSON.parse(item.state).template_id == this.template.gameBoardId);
    },
    show_question: function (text, ans) { 
      this.preview_text = 'Вопрос:\n' + text + '\n\nОтвет:\n' + ans;
      this.preview_question = true;
    },
    calc_sum_field: function () { 
      let tmp = 0;
      for (let field = 0; field < this.get_products_by_round().length; ++field) 
      {
        tmp = tmp + this.get_products_by_round()[field].numOfRepeating;
      }
      
      if(document.getElementById('total_field') && tmp > this.count_field)
        document.getElementById('total_field').style.color = 'red';
      else if(document.getElementById('total_field'))
        document.getElementById('total_field').style.color = 'white';
      return tmp;
    },
    count_field_16: function () { 
      this.count_field = 16;
      this.$emit('change-count', this.count_field);
      this.count_field_now();
    },
    count_field_20: function () { // смена количества полей в игре на 20
      this.count_field = 20;
      this.$emit('change-count', this.count_field);
      this.count_field_now();
    },
    count_field_24: function () { // смена количества полей в игре на 24
      this.count_field = 24;
      this.$emit('change-count', this.count_field);
      this.count_field_now();
    },
    count_field_now: function () { // подсчет текущего количества выбранных полей
      this.count_field_now_pole = 0;
      for (var field = 0; field < this.get_products_by_round().length; field++) {
        this.count_field_now_pole = this.count_field_now_pole + this.get_products_by_round()[field].numOfRepeating;
      }
      this.progress_bar()
    },
    progress_bar: function () { // обновление прогресс бара
      this.progress = this.count_field_now_pole / this.count_field * 100
      if (this.progress > 100) this.progress = 100
      this.progress = String(this.progress) + '%'
      if(document.getElementById('progress_1'))
        document.getElementById('progress_1').style = 'width:' + this.progress
    },
    expand_product: function (product, event) {
      product.visible_question = !product.visible_question;    
      let finded_target = event.target.className == 'polygon' ? event.target : event.target.firstElementChild;
      if(finded_target.style.transform == 'rotate(180deg)')
        finded_target.style = 'transform:rotate(0deg);';
      else
        finded_target.style = 'transform:rotate(180deg);';
    },
    get_readiable_type: function (type) {
      let mapping = new Map();
      mapping.set("TEXT", "Без выбора ответа").set("AUCTION", "Вопрос-аукцион").set("TEXT_WITH_ANSWERS", "С выбором ответа").set("MEDIA", "Вопрос с медиа фрагментом");
      return mapping.get(type);
    },
    delete_tmpl: async function () {
        await fetch(SERVER_PATH + "/gameBoard/delete/" + String(this.template.gameBoardId), {
            method: "DELETE",
            headers: {'Content-Type': 'application/json'}
        });
        this.$emit('back-to-templates');
    },
    cancel_tmpl: function () {
      this.$emit('to-questions');
    },
    custom_shuffle: function (array) {
      for (let i = array.length - 1; i > 0; i--) {
            let j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    },
  },
  mounted: function () {
    this.count_field = this.template.numFields;
    this.count_field_now();
    //this.fill_second_round_bar();
  },
  updated: function () {
    // this.fill_second_round_bar();
    this.$nextTick(function () {
    // Код, который будет запущен только после
    // отображения всех представлений
      this.fill_second_round_bar();
      if(!document.getElementById("Choice1"))
          return;
      this.count_field_now();
      if(this.count_field == 16)
      {
        document.getElementById("Choice1").checked = true;
        this.count_field_16();
      }
      if(this.count_field == 20)
      {
        document.getElementById("Choice2").checked = true;
        this.count_field_20();
      }
      if(this.count_field == 24)
      {
        document.getElementById("Choice3").checked = true;
        this.count_field_24();
      } 
    })
  }
}
</script>

<style scoped>
.radio_text_2{
  width: 68%;
  height: 80%;
  float: left;
  font-size: 2vw;
  margin-top: 0.7%;
  color: white;
}
.radio_text{
  width: 50%;
  height: 60%;
  float: left;
  font-size: 2vw;
  margin-top: 0.3%;
}
.button_themes{
  float: left;
  width: 5%;
  height: 60%;
  margin-left: 31.5%;
  margin-top: 1%;
  background-color:transparent;
  border: none;
}
.text_themes{
  float: left;
  width: 30%;
  height: 70%;
  font-size: 2vw;
  margin-left: 30%;
  margin-top: 1%;
  text-align: center;
}

 input:checked {
  border: 3px solid white;
  background: green;
}

.radio_fields_2{
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  margin-left:1% ;
  border-radius: 50%;
  width: 50%;
  height: 70%;
  background: white;
  border: 2px solid #999;
  transition: 0.2s all linear;
  outline: none;
  margin-right: 5px;
  margin-top: 1% ;
  position: relative;
  top: 4px;
}

.text_fields_themes{
 float: left;
 margin-top: 1.2%;
 width: 30%;
 height: 40%;
 text-align: center;
 font-size: 2vw;
 color: white;
}

.block_all_themes_filed{
   width: 99%;
   height: 82.5%;
   float: left;
}

#size_themes_2{
   float: left;
    height: 20%;
    width: 100%;
    margin-top: 0.7%;
}

.themes_themes_2{
   float: left;
   width: 100%;
   height: 100%;
   background-color: rgb(38, 146, 17);
   border-radius: 15px;
   border: solid 2.5px black;
}

.themes_themes{
   float: left;
   width: 98%;
   height: 100%;
   background-color: rgb(38, 146, 17);
   border-radius: 15px;
   border: solid 2.5px black;
}

.fields_number_progressbar{
   float: left;
   margin-right: 1%;
   margin-left: 1%;
   margin-top: 1%;
   font-size: 2vw;
}

.progressbar > span {
  border-radius: 15px;
  display: block;
  height: 100%;
  background-color: rgb(255,255,255);
  color: black;
  text-align: right;
  margin-right: 5%;
}

.progressbar {
  border-radius: 15px;
  margin-top: 1.5%;
  float: left;
  width: 40%;
  height: 40%;
  position: relative;
  background: rgba(255, 255, 255, 0.6);
}

.radio_fields{
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  margin-left:6% ;
  border-radius: 50%;
  width: 7%;
  height: 100%;
  background: white;
  border: 2px solid #999;
  transition: 0.2s all linear;
  outline: none;
  margin-right: 5px;
  margin-top: 1% ;
  position: relative;
  top: 4px;
}

.tex_themes_before{
    height: 8%;
    width: 98%;
    float: left;
    text-align: center;
    font-size: 2vw;
    margin-top: 0.7%;
}

#size_themes{
    height: 10%;
    width: 100%;
    margin-top: 0.7%;
}

.item_block_scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.item_block_scroll_2::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.item_block_scroll{
    height: 82%;
    width: 78%;
    float: left;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.item_block_scroll_2{
    height: 60%;
    width: 100%;
    float: left;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.item_block{
    height: 90%;
    width: 78%;
    float: left;
}

.text_fields{
  margin-top: 1% ;
  margin-left:2% ;
  height: 70%;
  width: 40%;
  float: left;
  font-size: 1.7vw;
}

.count_field{
  margin-top: 0.5%;
  height: 10%;
  width: 99%;
  float: left;
  background-color: rgba(33, 160, 56, 1);
  border-radius: 15px;
  border: 2.5px solid black;
  color: #ffffff;
  font-size: 30px;
  text-align: center;
}

.app{
  height: 100%;
  width: 100%;
}

.last_redaction{
    float: left;
    height: 90%;
    width: 10%;
    font-size: 1.6vw;
    margin-top: 0.5%;
    margin-left: 5%;
}

.name_quest2{
  float: left;
  height: 90%;
  width: 30%;
  font-size: 1.6vw;
  margin-top: 0.5%;
  margin-left: 5%;
}

.type_quest{
  float: left;
  height: 90%;
  width: 30%;
  margin-left: 5%;
  font-size: 1.6vw;
  margin-top: 0.5%;
}

.check_quest{
  float: left;
  height: 90%;
  width: 10%;
  margin-left: 5%;
  margin-top: 0.3%;
}

.check_quest_2{
  float: left;
  height: 90%;
  width: 90%;
  margin-top: 0.5%;
  margin-left: 5%;
  border-radius: 25%;
}

.quest_themes{
    float: left;
    border: 2px solid black;
    width: 98%;
    height: 70%;
    background-color: rgb(255, 255, 255);
    border-radius: 12px;
    font-size: 20px;
}

html {
    height: 100%;
    width: 100%;
}

body {
    width: 99%;
    height: 95%;
}

#app{
  height: 100%;
    width: 100%;
}

.telo {
    width: 99%;
    height: 90%;
    background: rgb(33, 160, 56);
}

.block_quest {
    width: 50%;
    height: 100%;
    float: left ;
}

.block_param {
    width: 49%;
    height: 100%;
    float: left ;
}

.block_background{
    width: 95%;
    height: 95%;
    float: left ;
    background: rgba(255, 255, 255, 0.7);
    border-radius: 5%;
    margin-top: 2%;
    margin-left: 2%;
}

.block_background_2{
    width: 95%;
    height: 25%;
    float: left ;
    background: rgba(255, 255, 255, 0.7);
    margin-top: 2%;
    margin-left: 2%;
    border-radius: 25px;
}

.block_background_3{
    width: 95%;
    height: 33%;
    float: left ;
    background: rgba(255, 255, 255, 0.7);
    margin-top: 2%;
    margin-left: 2%;
    border-radius: 25px;
}

.name_quest {
    width: 95%;
    height: 15%;
    float: left ;
    border-bottom: 2px solid green;
    text-align: center;
    margin-left: 2%;
    font-size: 24px;
    margin-top: 2%;
}

.telo_quest {
    width: 99%;
    height: 80%;
    float: left ;
    text-align: center;
    font-size: 24px;
    vertical-align:auto;
}

.name_answer {
    width: 95%;
    height: 25%;
    float: left ;
    border-bottom: 2px solid green;
    text-align: center;
    margin-left: 2%;
    font-size: 20px;
    margin-top: 2%;
}

.telo_answer {
    width: 99%;
    height: 60%;
    float: left ;
    text-align: center;
    font-size: 20px;
    vertical-align:auto;
}

.menu_left{
    float: left;
    width: 20%;
    text-align: center;
    height: 94%;
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
    font-size: 1.8vw;
}

#edit, #back, #delete{
    border-radius: 20px;
    padding-top: 4%;
    padding-bottom: 4%;
    margin-left: 10%;
    width: 80%;
    color: white;
    background-color: green;
    font-size: 1.2vw;
    font-weight: bold;
}

#edit{
    margin-top: 10%;
}

#delete{
    margin-top: 5%;
}

#back{
    margin-top: 100%;
}

#edit:hover, #back:hover, #delete:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

.polygon{
  height: 80%;
  width: 70%;
}

#group_delete{
  width: 70%;
  height: 70%;
  margin-left: 20%;
}

#delete_product_window{
    margin-top: 13%;
    width: 100%;
}

#delete_product_warning{
    width: 100%;
    font-size: 250%;
    text-align: center;
    margin-top: 20%;
    margin-left: 5%;
}

.delete_window_group_button{
    width: 80%;
    margin-left: 22%;
    height: 30%;
}

.button{
    float: left;
    width: 30%;
    background-color: green;
    text-align: center;
    padding-top: 1.5%;
    padding-bottom: 1.5%;
    color: white;
    font-size: 1.2vw;
    font-weight: bold;
    border-radius: 20px;
    margin-left: 4%;
    margin-right: 10%;
    margin-top: 5%;
}

.button:hover{
    box-shadow: 0 0 10px 100px orange inset;
}

#question_preview{
  position: absolute;
  background-color: white;
  border: 0.2vw solid black;
  left: 27.5%;
  width: 45%;
  top: 30%;
  height: 40%;
  border-radius: 1vw;
}

pre {
  white-space: pre-wrap; 
  word-wrap: break-word;
  font-family: inherit;
  font-size: 1.3vw;
  margin-left: 5%;
}

.scroll{
    height: 95%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    margin-top: 1%;
    width: 100%;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.grid{
    margin-top: 3%;
    margin-left: 5%;
    width: 90%;
    height: 80%;
}

.grid_element{
    margin-left: 2%;
    margin-right: 2%;
    margin-bottom: 5%;
    float: left;
    width: 20%;
    height: 22.5%;
    border: 2px solid black;
    border-radius: 20px;
    text-align: center;
    font-size: 1.7vw;
    line-height: 6.5vw;
    color: black;
    transition: transform .25s ease;
}

.grid_element:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.plus{
    font-size: 6vw;
    border: 2px solid silver;
    color: silver;
}

#game_list{
  height:80%;
}

.game_buttons{
  border-radius: 1vw;
  padding-top: 0.8%;
  padding-bottom: 0.8%;
  width: 15.85%;
  color: white;
  background-color: green;
  font-size: 1.1vw;
  font-weight: bold;
  position: absolute;
  text-align: center;
}

.game_buttons:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

img{
    transition: transform .25s ease;
}

img:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.questions{
  margin-top: 17.5%;
  margin-left: 1.5%;
  border: 0.15vw solid black;
  border-radius: 0.75vw;
  background: white;
  width: 10.15%;
  height: 18.45%;
  float: left;
  box-shadow: 0.4vw 0.2vw 0.2vw gray;
}

.start_sec_round{
  font-size: 1.5vw;
  text-align: center;
  vertical-align: center;
  line-height: 7.5vw;
}
</style>
