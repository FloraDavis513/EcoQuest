<template>
<div id="outer_field">
    <div class="first_line">
        <div class="start_one" id="start_1">
        <div class="product_main_one">Старт</div>
        </div>
        <div v-for="i in (game_settings.state.numFields / 4)" :key="i" class="start_themes_1" :style="calc_margin_hor()">
          <div v-if="!(game_settings.state.field_config[i - 1].logo)" class="product_themes_one" :id="'field_product_' + (i - 1)" :style="'color:white;background-color:' + game_settings.state.field_config[i - 1].colour + ';'" >
            {{ game_settings.state.field_config[i - 1].name }}
          </div>
          <img v-else :id="'field_product_' + (i - 1)" :src="img_path + game_settings.state.field_config[i - 1].logo  + '?' + Date.now()" style="width:124%;height:125%;" :alt="game_settings.state.field_config[i - 1].name" @change="update_poses" @show="update_poses">
        </div>
        <div class="start_two" id="start_2" :style="calc_margin_hor()">
        <div class="product_main_one">Старт</div>
        </div>
    </div>
    <div class="second_line">
        <div class="left_border">
            <div v-for="i in (game_settings.state.numFields / 4)" :key="i" class="start_themes_2" :style="calc_margin_ver_left(i)">
              <div v-if="!(game_settings.state.field_config[game_settings.state.numFields - i].logo)" class="product_themes_one" :id="'field_product_' + (game_settings.state.numFields - i)" :style="'color:white;background-color:' + game_settings.state.field_config[game_settings.state.numFields - i].colour + ';'">
                {{game_settings.state.field_config[game_settings.state.numFields - i].name }}
              </div>
              <img v-else :id="'field_product_' + (game_settings.state.numFields - i)" :src="img_path + game_settings.state.field_config[game_settings.state.numFields - i].logo  + '?' + Date.now()" style="width:138%;height:145%;" :alt="game_settings.state.field_config[game_settings.state.numFields - i].name">
            </div>
        </div>
        <div class="inner_field">
            <div v-if="current_question" id="tour_one_question_header">
              {{current_question.product.name}}
            </div>
            <div v-if="current_question" id="tour_one_question_sub_header">
              {{get_readiable_type()}}
            </div>
            <img v-show="turn == 1" :src="img_path + 'team_logo_' + String(Number(logos[0]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
            <img v-show="turn == 2" :src="img_path + 'team_logo_' + String(Number(logos[1]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
            <img v-show="turn == 0" :src="img_path + 'team_logo_' + String(Number(logos[show_last_team_logo()]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
            <img v-show="turn == 3" :src="img_path + 'team_logo_' + String(Number(logos[3]) + 1) + '.svg'" style="width:7.5%;height:7.5%;position:absolute;left:20%;top:20%;">
            <div id="tour_one_question_body">
              <textarea v-if="!current_question || current_question.question.media == null" id="greeting_message" rows="10" style="resize:none;border:none;width:100%;height:110%;margin-top:5%;font-size:1.35vw;" :value="current_question ? get_full_text() : game_settings.message" @blur="save_new_message" :readonly="logos.length > 0"></textarea>
              <img v-if="current_question && current_question.question.media != null && current_question.question.media.slice(-3) == 'png'" :src="get_preview_media()" style="height: 90%;width: 100%;margin-top: 5%;">
              <video v-if="current_question && current_question.question.media != null && current_question.question.media.slice(-3) == 'mp4'" :src="get_preview_media()" style="height: 90%;width: 100%;margin-top: 5%;" controls="controls"/>
            </div>
        </div>
        <div class="right_border">
            <div v-for="i in (game_settings.state.numFields / 4)" :key="i" class="start_themes_3" :style="calc_margin_ver_right(i)">
                <div v-if="!(game_settings.state.field_config[i - 1 + (game_settings.state.numFields / 4)].logo)" class="product_themes_one" :id="'field_product_' + (i - 1 + (game_settings.state.numFields / 4))" :style="'color:white;background-color:' + game_settings.state.field_config[i - 1 + (game_settings.state.numFields / 4)].colour + ';'">
                  {{ game_settings.state.field_config[i - 1 + (game_settings.state.numFields / 4)].name }}
                </div>
                <img v-else :id="'field_product_' + (i - 1 + (game_settings.state.numFields / 4))" :src="img_path + game_settings.state.field_config[i - 1 + (game_settings.state.numFields / 4)].logo + '?' + Date.now()" style="width:104%;height:145%;" :alt="game_settings.state.field_config[i - 1 + (game_settings.state.numFields / 4)].name">
              </div>
        </div>
    </div>
    <div class ="third_line">
        <div class="start_third" id="start_4">
        <div class="product_main_one">Старт</div>
        </div>
        <div v-for="i in (game_settings.state.numFields / 4)" :key="i" class="start_themes_4" :style="calc_margin_hor()">
          <div v-if="!(game_settings.state.field_config[3*game_settings.state.numFields/4 - i].logo)" class="product_themes_one" :id="'field_product_' + (3*game_settings.state.numFields/4 - i)" :style="'color:white;background-color:' + game_settings.state.field_config[3*game_settings.state.numFields/4 - i].colour + ';'">
            {{ game_settings.state.field_config[3*game_settings.state.numFields/4 - i].name }}
          </div>
          <img v-else :id="'field_product_' + (3*game_settings.state.numFields/4 - i)" :src="img_path + game_settings.state.field_config[3*game_settings.state.numFields/4 - i].logo + '?' + Date.now()" style="width:124%;height:125%;" :alt="game_settings.state.field_config[3*game_settings.state.numFields/4 - i].name">
        </div>
        <div class="start_four" id="start_3" :style="calc_margin_hor()">
        <div class="product_main_one">Старт</div>
        </div>
    </div>
</div>
</template>

<script>
import { SRC_PATH } from '../common_const.js'

export default {
  name: 'FirstRoundField',
  props: ['logos', 'game_settings', 'current_question', 'turn', 'teams'],
  data(){
    return {
        img_path: SRC_PATH,
        current_opacity: [1,1,1,1],
                
    }
  },
  methods: {
    show_last_team_logo: function () {
      if(this.teams == 2)
        return 1;
      if(this.teams == 3)
        return 3;
      if(this.teams == 4)
        return 2;
    },
    calc_margin_hor: function () {
        if(this.game_settings.state.numFields == 16)
            return "margin-left:7.1%";
        else if(this.game_settings.state.numFields == 20)
            return "margin-left:4.25%";
        else if(this.game_settings.state.numFields == 24)
            return "margin-left:2.25%";
    },
    calc_margin_ver_left: function (i) {
        if(this.game_settings.state.numFields == 16)
            return "margin-top:85%";
        else if(this.game_settings.state.numFields == 20 && i == 1)
            return "margin-top:45%";
        else if(this.game_settings.state.numFields == 20)
            return "margin-top:60%";
        else if(this.game_settings.state.numFields == 24 && i == 1)
            return "margin-top:25%";
        else if(this.game_settings.state.numFields == 24)
            return "margin-top:39%";
    },
    calc_margin_ver_right: function (i) {
        if(this.game_settings.state.numFields == 16 && i == 1)
            return "margin-top:75%";
        if(this.game_settings.state.numFields == 16)
            return "margin-top:75%";
        else if(this.game_settings.state.numFields == 20 && i == 1)
            return "margin-top:42%";
        else if(this.game_settings.state.numFields == 20)
            return "margin-top:53%";
        else if(this.game_settings.state.numFields == 24 && i == 1)
            return "margin-top:24%";
        else if(this.game_settings.state.numFields == 24)
            return "margin-top:35%";
    },
    save_new_message: function () {
        this.$emit('new-message', document.getElementById('greeting_message').value);
    },
    get_readiable_type: function () {
        let mapping = new Map();
        mapping.set("TEXT", "без выбора ответа").set("AUCTION", "вопрос-аукцион").set("TEXT_WITH_ANSWERS", "с выбором ответа").set("MEDIA", "вопрос с медиа фрагментом");
        return mapping.get(this.current_question.question.type);
    },
    get_preview_media: function () {
        if(!this.current_question.question.media)
            return null;
        const regex = /media\d+.\w+/g;
        const found = this.current_question.question.media.match(regex);
        return SRC_PATH + found + '?' + Date.now();
    },
    get_full_text: function () {
        let full_text = this.current_question.question.text;
        let answers;
        if(this.current_question.question.answers == null || (typeof this.current_question.question.answers != "object"))
          return full_text;
        answers = JSON.parse(this.current_question.question.answers);
        if(answers.AllAnswers == null || answers.CorrectAnswers == null)
          return full_text;
        if(answers.AllAnswers.length == answers.CorrectAnswers)
          return full_text;
        let all_answers = [];
        answers.AllAnswers.forEach(item => all_answers.push(item));
        this.custom_shuffle(all_answers);
        all_answers.forEach(item => full_text += ('\n* ' + item) )
        return full_text;
    },
    custom_shuffle: function (array) {
      for (let i = array.length - 1; i > 0; i--) {
            let j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    },
  },
  mounted: function() {
    this.$emit('update-field');
    if(this.logos.length > 0)
        document.getElementById('greeting_message').style = 'resize:none;border:none;width:100%;height:110%;margin-top:5%;font-size:1.35vw;outline:none;';
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
  /* text-indent: 3%; */
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
