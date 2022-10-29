<template>
<div id="outer_field">
    <div class="first_line">
        <div class="start_one" id="start_one">
        <div class="product_main_one">Старт</div>
        </div>
        <div v-for="i in (current_template.numFields / 4)" :key="i" class="start_themes_1" :style="calc_margin_hor()">
          <div v-if="!(field_config[i - 1].name in images)" class="product_themes_one" :id="'field_product_' + (i - 1)" :style="field_config[i - 1].colour">
            {{ field_config[i - 1].name }}
          </div>
          <img v-else :id="'field_product_' + (i - 1)" :src="images[field_config[i - 1].name]" style="width:124%;height:125%;" :alt="field_config[i - 1].name">
        </div>
        <div class="start_two" id="start_two" :style="calc_margin_hor()">
        <div class="product_main_one">Старт</div>
        </div>
    </div>
    <div class="second_line">
        <div class="left_border">
            <div v-for="i in (current_template.numFields / 4)" :key="i" class="start_themes_2" :style="calc_margin_ver_left(i)">
              <div v-if="!(field_config[current_template.numFields - i].name in images)" class="product_themes_one" :id="'field_product_' + (current_template.numFields - i)" :style="field_config[current_template.numFields - i].colour">
                {{field_config[current_template.numFields - i].name }}
              </div>
              <img v-else :id="'field_product_' + (current_template.numFields - i)" :src="images[field_config[current_template.numFields - i].name]" style="width:138%;height:145%;" :alt="field_config[current_template.numFields - i].name">
            </div>
        </div>
        <div class="inner_field">
            <div v-if="question[1] != 'Приветствуем Вас в игре по ЭкоСистеме Сбера'" id="tour_one_question_header">
            {{question[2]}}
            </div>
            <div v-if="question[1] != 'Приветствуем Вас в игре по ЭкоСистеме Сбера'" id="tour_one_question_sub_header">
            {{question[1]}}
            </div>
            <div v-else id="tour_one_question_header" style="margin-bottom:8%;">
            {{question[1]}}
            </div>
              <img v-show="turn == 0" :src="img_path + 'team_logo_' + String(Number(logos[0]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
              <img v-show="turn == 1" :src="img_path + 'team_logo_' + String(Number(logos[1]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
              <img v-show="turn == 3" :src="img_path + 'team_logo_' + String(Number(logos[2]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
              <img v-show="turn == 2" :src="img_path + 'team_logo_' + String(Number(logos[3]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
            <div id="tour_one_question_body">
              <div class="scroll">
                <pre>{{ question[0] }}</pre>
              </div>
            </div>
        </div>
        <div class="right_border">
            <div v-for="i in (current_template.numFields / 4)" :key="i" class="start_themes_3" :style="calc_margin_ver_right(i)">
                <div v-if="!(field_config[i - 1 + (current_template.numFields / 4)].name in images)" class="product_themes_one" :id="'field_product_' + (i - 1 + (current_template.numFields / 4))" :style="field_config[i - 1 + (current_template.numFields / 4)].colour">
                  {{ field_config[i - 1 + (current_template.numFields / 4)].name }}
                </div>
                <img v-else :id="'field_product_' + (i - 1 + (current_template.numFields / 4))" :src="images[field_config[i - 1 + (current_template.numFields / 4)].name]" style="width:104%;height:145%;" :alt="field_config[i - 1 + (current_template.numFields / 4)].name">
              </div>
        </div>
    </div>
    <div class ="third_line">
        <div class="start_third" id="start_four">
        <div class="product_main_one">Старт</div>
        </div>
        <div v-for="i in (current_template.numFields / 4)" :key="i" class="start_themes_4" :style="calc_margin_hor()">
          <div v-if="!(field_config[3*current_template.numFields/4 - i].name in images)" class="product_themes_one" :id="'field_product_' + (3*current_template.numFields/4 - i)" :style="field_config[3*current_template.numFields/4 - i].colour">
            {{ field_config[3*current_template.numFields/4 - i].name }}
          </div>
          <img v-else :id="'field_product_' + (3*current_template.numFields/4 - i)" :src="images[field_config[3*current_template.numFields/4 - i].name]" style="width:124%;height:125%;" :alt="field_config[3*current_template.numFields/4 - i].name">
        </div>
        <div class="start_four" id="start_three" :style="calc_margin_hor()">
        <div class="product_main_one">Старт</div>
        </div>
    </div>
</div>
</template>

<script>
import { SRC_PATH } from '../common_const.js'

export default {
  name: 'FirstRoundField',
  props: ['logos', 'current_template', 'field_config', 'question'],
  data(){
    return {
        img_path: SRC_PATH,
        collapse: true,
        current_opacity: [1,1,1,1],
        images: {'ДеливериКлаб': SRC_PATH + 'деливериклаб.png',
                'СберЕАптека':SRC_PATH + 'сбереаптека.png',
                'СберЗвук':SRC_PATH + 'сберзвук.png',
                'Okko':SRC_PATH + 'окко.png',
                'Самокат':SRC_PATH + 'самокат.png',
                'СберДиск':SRC_PATH + 'сбердиск.png',
                'СберЗдоровье':SRC_PATH + 'сберздоровье.png',
                'СберЛогистика':SRC_PATH + 'сберлогистика.png',
                'СберМаркет':SRC_PATH + 'сбермаркет.png',
                'СММ':SRC_PATH + 'смм.png',
                'СберСтрахование':SRC_PATH + 'сберстрахование.png',
                'СберМобайл':SRC_PATH + 'сбермобайл.png'}
                
    }
  },
  methods: {
    calc_margin_hor: function () {
        if(this.current_template.numFields == 16)
            return "margin-left:7.1%";
        else if(this.current_template.numFields == 20)
            return "margin-left:4.25%";
        else if(this.current_template.numFields == 24)
            return "margin-left:2.25%";
    },
    calc_margin_ver_left: function (i) {
        if(this.current_template.numFields == 16)
            return "margin-top:85%";
        else if(this.current_template.numFields == 20 && i == 1)
            return "margin-top:45%";
        else if(this.current_template.numFields == 20)
            return "margin-top:60%";
        else if(this.current_template.numFields == 24 && i == 1)
            return "margin-top:25%";
        else if(this.current_template.numFields == 24)
            return "margin-top:39%";
    },
    calc_margin_ver_right: function (i) {
        if(this.current_template.numFields == 16 && i == 1)
            return "margin-top:75%";
        if(this.current_template.numFields == 16)
            return "margin-top:75%";
        else if(this.current_template.numFields == 20 && i == 1)
            return "margin-top:42%";
        else if(this.current_template.numFields == 20)
            return "margin-top:53%";
        else if(this.current_template.numFields == 24 && i == 1)
            return "margin-top:24%";
        else if(this.current_template.numFields == 24)
            return "margin-top:35%";
    },
    set_current_turn: function (i) {
        this.turn = i;
    },
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.first_line{
  width: 100%;
  height: 10%;
}
.third_line{
  width: 100%;
  height: 10%;
}
.second_line{
  width: 100%;
  height: 80%;
}
.start_one{
    margin-top: 1%;
    margin-left: 1%;
    height: 100%;
    width: 10%;
    float: left;
}
.start_two{
    margin-top: 1%;
    margin-left: 5.6%;
    height: 100%;
    width: 10%;
    float: left;
}
.start_third{
    margin-top: -3.5%;
    margin-left: 1%;
    height: 100%;
    width: 10%;
  float: left;
}
.start_four{
    margin-top: -3.5%;
    margin-left: 5%;
    height: 100%;
    width: 10%;
  float: left;
}
.start_themes_1{
    margin-left: 5.6%;
    height: 100%;
    width: 10%;
    float: left;
    z-index:2;
    margin-top: 1%;
}
.start_themes_2{
    margin-top: 75%;
    height: 12%;
    width: 100%;
    float: left;
    z-index:2;
    margin-left: 12%;
}
.start_themes_3{
    margin-top: 65%;
    height: 12%;
    width: 120%;
    float: left;
    z-index:2;
    margin-left: -17%;
}
.start_themes_4{
    margin-left: 5.5%;
    height: 100%;
    width: 10%;
    float: left;
    z-index:2;
    margin-top: -3.5%;
}
.product_main_one{
    border-radius: 1vw;
    height: 12%;
    width: 12%;
    background-color: white;
    border: solid black 0.2vw;
    position: absolute;
    text-align: center;
    line-height: 450%;
    float: left;
    z-index:2;
    font-size: 1.1vw;
}
.product_themes_one{
    border-radius: 1vw;
    height: 12%;
    width: 12%;
    background-color: red;
    border: solid black 0.15vw;
    position: absolute;
    text-align: center;
    line-height: 4.75vw;
    float: left;
    z-index:2;
    font-size: 0.9vw;
}

#tour_one_question_header{
  margin-top: 5%;
  text-align: center;
  font-size: 1.75vw;
  color: green;
  height: 6%;
  width: 100%;
}

#tour_one_question_sub_header{
  margin-top: 1%;
  text-align: center;
  font-size: 1.35vw;
  color: gray;
  height: 6%;
  width: 100%;
}

pre {
  white-space: pre-wrap; 
  word-wrap: break-word;
  font-family: inherit;
}

#tour_one_question_body{
  text-align: justify;
  margin-left: 5%;
  font-size: 1.35vw;
  height: 80%;
  width: 90%;
  text-indent: 3%;
}

.right_border{
  float: left;
  height: 90%;
  width: 10%;
}

.left_border{
  float: left;
  height: 90%;
  width: 9%;
}

.inner_field{
  margin-left: 7%;
  height: 85%;
  width: 67.6%;
  border-radius: 2.5vw;
  float: left;
  margin-top: 5%;
  background-color: white;
  margin-bottom: -15%;
  margin-right: 4.5%;
  border: solid black 0.15vw;
}

.product{
    border-radius: 50%;
    height: 10%;
    width: 5%;
    background-color: white;
    border: solid black 0.2vw;
    position: absolute;
    text-align: center;
    line-height: 400%;
}

#outer_field{
    height: 90%;
    width: 46%;
    background-color: green;
    background-color: white;
    border-radius: 1vw;
    border: solid black 0.2vw;
    position: absolute;
    left: 27%;
    top: 8%;
    box-shadow: 1vw 0.5vw 0.2vw gray;
}

#tour_one_question_logo{
  width: 100%;
  height: 100%;
  z-index: 0;
}

.scroll{
    height: 98%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}
</style>
