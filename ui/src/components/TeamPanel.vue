<template>
  <div v-if="players.length > 0" id="block" class="block">
    <div v-if="visible==2" class="all_cont">
      <div class="center_item_team">
         <div class="column_img" style="float:left;">
            <img id="logo" :src="img_path + 'team_logo_' + String(Number(logo) + 1) + '.svg'">
        </div>
        <div v-if="team_number == 'team_1'" class="name_team" @click="give_puzzle('team_1')">{{name}}</div>
        <div v-if="team_number == 'team_2'" class="name_team" @click="give_puzzle('team_2')">{{name}}</div>
        <div v-if="team_number == 'team_4'" class="name_team" @click="give_puzzle('team_4')">{{name}}</div>
        <div v-if="team_number == 'team_3'" class="name_team" @click="give_puzzle('team_3')">{{name}}</div>
        <div class="change_label">
          <button class="button" v-on:click="visible1" style="height:100%"><img src="@/assets/menu.png" alt="Кнопка «button»" style="height:100%"></button>
        </div>
        <div id="group_help">
            <div class="help_border" style="margin-left:20%">
              <img class="help" v-show="helps[0]" @click="up_hand" src="@/assets/help_master.png">
            </div>
            <div class="help_border" style="margin-right:15%">
              <img class="help" v-show="helps[1]" @click="helps[1] = false" src="@/assets/help_player.png">
            </div>
            <div class="help_border" style="margin-left:20%">
              <img class="help" v-show="helps[2]" @click="replace_question" src="@/assets/replace_q.png">
            </div>
            <div class="help_border" style="margin-right:15%">
              <img class="help" v-show="helps[3]" @click="replace_field" src="@/assets/replace_f.png">
            </div>
        </div>
      </div>
      <div class="team_score">
          <div class="puzzle_1">
            1 раунд
          </div>
          <div class="puzzle_2">
            {{puzzle}}
          </div>
          <div class="puzzle_3">
            /
          </div>
          <div class="puzzle_4">
            {{unique_products.length}}
          </div>
          <div class="points_1">
            2 раунд
          </div>
          <div class="points_2">
            {{score}}
          </div>
      </div>
      <div class="bottom_item_team">
        <div class="themes1">
            <div v-for="(item, index) in unique_products" class="themes4" :style="themes_sber[index].color" :key="index">
            </div>
        </div>
        <div  class="button_themes">
          <img id="dots" src="@/assets/dots.png" alt="Кнопка «button»" v-on:click="visible3">
        </div>
      </div>
      
    </div>
    <div v-else-if="visible==1" class="all_cont">
      <div class="top_item_team">
        <div class="name_team" style="width:70%;">
          Состав команды
        </div>
        <div class="change_label" style="height:82%;">
          <button class="button" v-on:click="visible2" style="height:100%"><img src="@/assets/menu.png" alt="Кнопка «button»" style="height:100%"></button>
        </div>
      </div>
      <div class="lobby_team" id="lobby_team_1">
        <div class="scroll">
            <div id="list_team_1" class="one_player" v-for="(option, index) in team" :key="index" :index="index">
                <input type="text" :value="option" class="wait_users" @blur="save_edit">
                <div id="delete_player" @click="team.pop()">x</div>
            </div>
            <div id="plus_player" @click="team.push('Новый игрок')">+</div>
        </div>
      </div>
    </div>
    <div v-else class="all_cont">
      <div class="top_item_team">
        <div v-if="team_number == 'team_1'" class="name_team" @click="give_puzzle('team_1')" style="width:50%;">Команда 1</div>
        <div v-if="team_number == 'team_2'" class="name_team" @click="give_puzzle('team_2')" style="width:50%;">Команда 2</div>
        <div v-if="team_number == 'team_3'" class="name_team" @click="give_puzzle('team_4')" style="width:50%;">Команда 4</div>
        <div v-if="team_number == 'team_4'" class="name_team" @click="give_puzzle('team_3')" style="width:50%;">Команда 3</div>
        <div class="change_label" style="height:82%;margin-left:23%;">
          <button class="button" v-on:click="visible2" style="height:100%"><img src="@/assets/menu.png" alt="Кнопка «button»" style="height:100%"></button>
        </div>
      </div>
      <div class="themes2">
        <div v-for="(item, index) in unique_products" class="themes3" :id="team_number + '_info_puzzle_' + String(index)" :style="themes_sber[index].color" :key="index">
          {{ item.name }}
        </div>
      </div>
      <div class="button_themes2">
          <button id="dots_parent" class="button" v-on:click="visible2" style="height:100%;width:90%;"><img id="dots" src="@/assets/dots.png" alt="Кнопка «button»"></button>
      </div>
      <div class="team_score">
        <div class="points_1">
            Баллы:
          </div>
          <div class="points_2">
            {{score}}
          </div>
          <div class="puzzle_1">
            Пазлы:
          </div>
          <div class="puzzle_2">
            {{puzzle}}
          </div>
          <div class="puzzle_3">
            /
          </div>
          <div class="puzzle_4">
            {{unique_products.length}}
          </div>
      </div>
    </div>
    </div>
</template>

<script>
import { SRC_PATH } from '../common_const.js'

export default {
  name: 'TeamPanel',
  props:['logo', 'name', 'team_number', 'puzzle', 'score', 'players', 'unique_products', 'tour', 'question'],
  data: function () {
    return {
      img_path: SRC_PATH,
      visible: 2,
      themes_sber: [
          {product:'test', color:'background:white;', font:'line-height:200%;'},
          {product:'test2', color:'background:white;', font:'line-height:200%;'},
          {product:'test3', color:'background:white;', font:'line-height:200%;'}, 
          {product:'СберДиск', color:'background:white;', font:'line-height:200%;'}, 
          {product:'СберБокс', color:'background:white;', font:'line-height:200%;'}, 
          {product:'СберМаркет', color:'background:white;', font:'font-size:78%;line-height:250%;'}, 
          {product:'ДеливериКлаб', color:'background:white;', font:'font-size:65%;line-height:310%;'}, 
          {product:'Самокат', color:'background:white;', font:'line-height:200%;'}, 
          {product:'СитиМобил', color:'background:white;', font:'font-size:82%;line-height:240%;'}, 
          {product:'СберПрайм', color:'background:white;', font:'font-size:86%;line-height:230%;'}, 
          {product:'СберПрайм+', color:'background:white;', font:'font-size:75%;line-height:250%;'}, 
          {product:'СберМобайл', color:'background:white;', font:'font-size:78%;line-height:250%;'}, 
          {product:'СберЕАптека', color:'background:white;', font:'font-size:72%;line-height:280%;'}, 
          {product:'ДомКлик', color:'background:white;', font:'line-height:200%;'}, 
          {product:'СберЗдоровье', color:'background:white;', font:'font-size:70%;line-height:290%;'}, 
          {product:'СберМегаМаркет', color:'background:white;', font:'font-size:56%;line-height:360%;'},
          ],
      team: ['Игрок 1'],
      helps: [true, true, true, true]
    }
  },
  methods: {
    visible1 () { this.visible = 1 },
    visible2 () { this.visible = 2;  },
    visible3 () { this.visible = 3;  },
    action(product, color) {
      for(var i = 0; i < this.themes_sber.length; ++i)
      {
          if(this.themes_sber[i].product == product)
              this.themes_sber[i].color = color;
      }
    },
    check_color(product) {
      for(var i = 0; i < this.themes_sber.length; ++i)
      {
          if(this.themes_sber[i].product == product)
              return this.themes_sber[i].color;
      }
    },
    fill_themes_sber(unique_products) {
      this.themes_sber.length = 0;
      for( let i = 0; i < unique_products.length; ++i)
        this.themes_sber.push({product: unique_products[i].name, color: 'background:white;'});
    },
    up_hand () {
      var current_team = this.team_number;
      var secs = 10;
      
      var timer = setInterval(tick,1000)
      function tick(){
          (--secs);
          if(secs % 2 == 0)
              document.getElementById(current_team).style.border = "0.1vw rgba(255, 255, 255, 0) solid";
          else
              document.getElementById(current_team).style.border = "0.4vw red solid";
          if(secs == 0)
              clearInterval(timer);
      }
    },
    give_puzzle(team) {
        if(this.tour == 1)
        {
            this.$emit('give-puzzle', team, this.question[2]);
        }
        else
            this.$emit('give-score', team);
        
    },
    replace_question() {
        this.helps[2] = false;
        this.$emit('replace-question');
    },
    replace_field() {
        this.helps[3] = false;
        this.$emit('replace-field');
    }
  },
  created () {
      if(this.players)
          this.team = this.players;
    },
    updated () {
      let i = 0;
      let cur_puzzle = document.getElementById(this.team_number + '_info_puzzle_' + String(i));
      if( !cur_puzzle )
        return;
      while(cur_puzzle)
      {
        cur_puzzle.style.fontSize = String(1.4 * cur_puzzle.offsetWidth / cur_puzzle.innerText.length) + 'px';
        ++i;
        cur_puzzle = document.getElementById(this.team_number + '_info_puzzle_' + String(i));
      }
      
    }
}
</script>

<style scoped>
.help{
  width: 90%;
  height: 90%;
  margin-top: 5%;
  margin-left: 5%;
}
.themes4{
  margin-top: 2%;
  margin-left: 1%;
  width: 10%;
  height: 40%;
  float: left ;
  background-color: white;
  border: solid 0.15vw black;
  border-radius: 0.3vw;
}
.themes3{
  margin-top: 2%;
  margin-left: 3%;
  width: 20%;
  height: 20%;
  border: 1px solid black;
  float: left ;
  color: black;
  border-radius: 15%;
  text-align: center;
  font-size: 1vw;
  line-height: 2.5vw;
}
.button_themes2{
  width: 10%;
height: 15%;

  align-content: center;
  float: left ;
  margin-top: 20% ;
}
.themes2{
  width: 90%;
  height: 90%;

  float: left ;
}
.button_themes{
  width: 7%;
  height: 50%;
  float: left ;
  margin-top: 5.5%;
}
.themes1{
  width: 89%;
  height: 100%;
  margin-left: 3%;
  float: left ;
}
.block{
  width: 22%;
  height: 35%;
  background-color: white;
  border-radius: 1.5vw;
  position: absolute;
  border: 0.1vw black solid;
  box-shadow: 0.7vw 0.4vw 0.2vw gray;
}
.all_cont{
  margin-top: 2%;
  width: 100%;
  height: 80%;
}
.top_item_team{
  width: 90%;
  height: 17%;
  margin-left: 8%;
}
.question{
  width: 15%;
  height: 90%;
  float: left ;
  margin-top: 2%;
}
.button{
  margin-top: 10%;
  margin-left: 10%;
  width: 50%;
}
.name_team{
  width: 38%;
  height: 25%;
  text-align: center;
  margin-top: 1.5%;
  float: left;
  font-size: 1.5vw;
  color: black;
  margin-left: 10%;
}

.name_team:hover {
  transform: scale(1.15); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.change_label{
  width: 15%;
  height: 27%;
  margin-left: 1%;
  float: left;
}

.center_item_team{
  width: 100%;
  height: 50%;
  margin-top: 5%;
}

.column_img{
  width: 30%;
  height: 100%;
  float: left;
  background: white;
  margin-left: 5%;
}

#logo{
  width: 120%;
  height: 175%;
  transform: none;
  margin-top: -25%;
}

#group_help{
  width: 65%;
  height: 100%;
  float: left;
  margin-top: 3%;
}

.help_border{
  border: solid 0.2vw rgb(210 , 210 , 210);
  height: 40%;
  width: 25%;
  float: left;
  border-radius: 0.5vw;
}

.bottom_item_team{
  width: 100%;
  height: 28%;
  margin-left: auto;
  margin-right: auto;
}

.team_score{
  width: 100%;
  height: 10%;
  margin-top: 1.5%;
  margin-left: 4%;
  margin-right: auto;
}

.points_1{
  width: 25%;
  height: 95%;
  float: left;
  font-size: 1.1vw;
  color: black;
  margin-left: 25%;
}

.points_2{
  width: 5%;
  height: 95%;
  float: left ;
  font-size: 1.2vw;
  color: black;
}

.puzzle_1{
  width: 25%;
  height: 95%;
  float: left;
  font-size: 1.1vw;
  color: black;
}

.puzzle_2{
  width: 3%;
  height: 95%;
  float: left ;
  font-size: 1.1vw;
  color: black;
}

.puzzle_3{
  width: 1%;
  height: 95%;
  float: left;
  font-size: 1.1vw;
  color: black;
}
.puzzle_4{
  width: 1%;
  height: 95%;
  float: left;
  font-size: 1.1vw;
  color: black;
  margin-left: 1.75%;
}

button{
  background-color:transparent;
  border: none;
}

#dots{
    height: 100%;
    width: 120%;
}

.lobby_team{
    width: 95%;
    height: 90%;
    margin-left: 2.5%;
    margin-top: 3%;
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

.one_player{
    width: 100%;
}

.wait_users{
    margin-top: 2%;
    margin-left: 2%;
    width: 39%;
    border: solid black 2px;
    border-radius: 10px;
    text-align: center;
    font-size: 0.9vw;
    padding-top: 1.5%;
    padding-bottom: 1.5%;
    float: left;
}

#delete_player{
    float: left;
    width: 5%;
    margin-top: 4%;
    margin-left: 2%;
    font-size: 1.3vw;
}

#plus_player{
    margin-top: 2%;
    margin-left: 10%;
    width: 80%;
    text-align: center;
    font-size: 1.5vw;
    font-weight: bold;
    float: left;
}

img{
  transition: transform .25s ease;
}

img:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}
</style>
