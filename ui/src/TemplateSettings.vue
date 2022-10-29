<template>
<div class="app">
<AdminHeader  />
<ProductMenu2 @select-question= "visible1" @select-themes= "visible2" @show-game= "visible5"  />
    <div style="height:5%;width:100%;background-color:black;">
      1
    </div>
    <div v-if="visible==2" class="item_block_scroll">
          <div v-for="(product,index) in products" :key="index" id="size_themes">
            <div class="themes_themes" :style="product.color" >
                <div class="text_themes" >
                  {{product.text}}
                </div>
                <button class="button_themes" v-on:click="product.visible_question=!product.visible_question"><img src="./assets/Polygon_1.png"></button>
            </div>
            <div v-for="(question,index) in product.questions" :key="index" class="quest_themes"  v-show="product.visible_question">
                <div class="name_quest2">
                  {{question.text}}
                </div>
                <div class="type_quest">
                  {{question.type}}
                </div>
                <div class="check_quest">
                    <input class="check_quest_2" type="checkbox" v-on:click="product.question.Need_quest =! product.question.Need_quest">
                </div>
            </div>
          </div>
    </div>
    <div v-if="visible==2" class="item_block">
      <div class="count_field">
          <div class="text_fields">
            Число игровых полей
          </div>
            <form>
              <div class="radio_text">
              <input type="radio" id="Choice1" class="radio_fields" checked
              name="Fields_Count" value="email"  v-on:click="count_field_16">
              <label>16</label>

              <input type="radio" id="Choice2" class="radio_fields"
              name="Fields_Count" value="phone" v-on:click="count_field_20">
              <label>20</label>

              <input type="radio" id="Choice3" class="radio_fields"
              name="Fields_Count" value="mail" v-on:click="count_field_24">
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
          <label class="fields_number_progressbar">{{count_field}}</label>
      </div>
      <div class="tex_themes_before">
        Укажите количество повторений темы на игровом поле
      </div>
      <div class="item_block_scroll_2" >
          <div class="block_all_themes_filed" >
            <div v-for="(product,index) in products" :key="index" id="size_themes_2">
                  <div class="themes_themes_2" :style="product.color"  >
                      <div class="text_fields_themes" >
                        {{product.text}}
                      </div>
                      <form>
                        <div class="radio_text_2">
                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="email" v-on:click="product.count = 1, count_field_now()">
                          <label >1</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="phone" v-on:click="product.count = 2, count_field_now()">
                          <label >2</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="mail" v-on:click="product.count = 3, count_field_now()">
                          <label >3</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="email" v-on:click="product.count = 4, count_field_now()">
                          <label >4</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="phone" v-on:click="product.count = 5, count_field_now()">
                          <label >5</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="mail" v-on:click="product.count = 6, count_field_now()">
                          <label >6</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="email"  v-on:click="product.count = 7, count_field_now()">
                          <label >7</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="phone"  v-on:click="product.count = 8, count_field_now()">
                          <label >8</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="mail"  v-on:click="product.count = 9, count_field_now()">
                          <label >9</label>

                          <input type="radio" class="radio_fields_2"
                          name="Fields_Count" value="mail"  v-on:click="product.count = 10 , count_field_now()">
                          <label >10</label>
                        </div>
                      </form>
                  </div>
            </div>
          </div>
      </div>
    </div>
 </div>
</template>

<script>
import AdminHeader from './components/AdminHeader.vue'
import ProductMenu2 from './components/ProductMenu2.vue'

export default {
  name: 'App',
  data () {
    return {
      visible: 1,
      name_template: '',
      count_field: 16,
      count_field_now_pole: 1,
      progress: 0,
      products: [
        { text: '1',
          value: '1',
          color: 'color:black;',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 1', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 2', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 3', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 4', Need_quest: false }
          ]
        },
        { text: '2',
          value: '2',
          color: 'color:black;',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false },
            { id: '5', text: 'Вопрос 5', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false }
          ]
        },
        { text: 'СберАптека',
          value: '3',
          color: 'color:white;background:red',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 1', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 2', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 3', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 4', Need_quest: false }
          ]
        },
        { text: 'ДомКлик',
          value: '4',
          color: 'color:white;background:purple',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false }
          ]
        },
        { text: 'ДомКлик',
          value: '4',
          color: 'color:white;background:purple',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false }
          ]
        },
        { text: 'ДомКлик',
          value: '4',
          color: 'color:white;background:purple',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false }
          ]
        },
        { text: 'ДомКлик',
          value: '4',
          color: 'color:white;background:purple',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false }
          ]
        },
        { text: 'ДомКлик',
          value: '4',
          color: 'color:white;background:purple',
          count: 1,
          visible_question: true,
          questions: [
            { id: '1', text: 'Вопрос 1', type: 'С выбором ответа', wording: 'Какой-то вопрос 5', Need_quest: false },
            { id: '2', text: 'Вопрос 2', type: 'Без выбора ответа', wording: 'Какой-то вопрос 6', Need_quest: false },
            { id: '3', text: 'Вопрос 3', type: 'Вопрос-аукцион', wording: 'Какой-то вопрос 7', Need_quest: false },
            { id: '4', text: 'Вопрос 4', type: 'Вопрос с медиа фрагментом', wording: 'Какой-то вопрос 8', Need_quest: false }
          ]
        }
      ]
    }
  },
  components: {
    AdminHeader,
    ProductMenu2
  },
  methods: {
    visible1: function () { // переключение на вкладку выбор вопросов
      this.visible = 1
      this.count_field_now()
    },
    visible2: function () { // переключение на вкладку выбор тем
      this.visible = 2
      this.count_field_now()
    },
    visible5: function () { // переключение на вкладку выбор тем
      this.visible = 5;
    },
    count_field_16: function () { // смена количества полей в игре на 16
      this.count_field = 16
      this.count_field_now()
    },
    count_field_20: function () { // смена количества полей в игре на 20
      this.count_field = 20
      this.count_field_now()
    },
    count_field_24: function () { // смена количества полей в игре на 24
      this.count_field = 24
      this.count_field_now()
    },
    count_field_now: function () { // подсчет текущего количества выбранных полей
      this.count_field_now_pole = 0
      for (var field = 0; field < this.products.length; field++) {
        this.count_field_now_pole = this.count_field_now_pole + this.products[field].count
      }
      this.progress_bar()
    },
    progress_bar: function () { // обновление прогресс бара
      this.progress = this.count_field_now_pole / this.count_field * 100
      if (this.progress > 100) this.progress = 100
      this.progress = String(this.progress) + '%'
      document.getElementById('progress_1').style = 'width:' + this.progress
    }
  },
  mounted: function () {
    this.$nextTick(function () {
    // Код, который будет запущен только после
    // отображения всех представлений
      this.count_field_now()
    })
  },
  updated: function () {
    this.$nextTick(function () {
    // Код, который будет запущен только после
    // отображения всех представлений
      this.count_field_now()
    })
  }
}
</script>

<style>
.radio_text_2{
font-size: 100%;
width: 68%;
height: 80%;
float: left;
font-size: 250%;
margin-top: 0.7%;
color: white;
}
.radio_text{
font-size: 100%;
width: 50%;
height: 60%;
float: left;
font-size: 150%;
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
font-size: 200%;
color:#ffffff;
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
  width: 40px;
  height: 40px;
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
 font-size: 250%;
 color: white;
}
.block_all_themes_filed{
   width: 99%;
   height: 70%;
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
   font-size: 40px;
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
  width: 40px;
  height: 40px;
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
    font-size: 250%;
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
    height: 90%;
    width: 78%;
    float: left;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}
.item_block_scroll_2{
    height: 70%;
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

.name_quest2{
float: left;
height: 90%;
width: 30%;
font-size: 150%;
margin-top: 0.5%;
margin-left: 5%;
}
.type_quest{
float: left;
height: 90%;
width: 30%;
font-size: 20px;
margin-left: 15%;
font-size: 150%;
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
margin-left: 45%;
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
    font-size: 180%;
}

#edit, #back, #delete{
    border-radius: 20px;
    padding-top: 4%;
    padding-bottom: 4%;
    margin-left: 10%;
    width: 80%;
    color: white;
    background-color: green;
    font-size: 120%;
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
</style>
