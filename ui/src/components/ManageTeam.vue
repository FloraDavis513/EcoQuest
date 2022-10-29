<template>
<div class="lobby_team">
    <img v-if="view != 'menu' && team_number == 'team_1'" src="@/assets/lobby_back.png" alt="" style="position:absolute;left:25%;top:9%;width:1.75%;" @click="view = 'menu'">
    <img v-if="view != 'menu' && team_number == 'team_2'" src="@/assets/lobby_back.png" alt="" style="position:absolute;left:53%;top:9%;width:1.75%;" @click="view = 'menu'">
    <img v-if="view != 'menu' && team_number == 'team_3'" src="@/assets/lobby_back.png" alt="" style="position:absolute;left:25%;top:55.75%;width:1.75%;" @click="view = 'menu'">
    <img v-if="view != 'menu' && team_number == 'team_4'" src="@/assets/lobby_back.png" alt="" style="position:absolute;left:53%;top:55.75%;width:1.75%;" @click="view = 'menu'">
    <input v-if="team_number == 'team_1'" id="lobby_team_head_1" maxlength="10" class="team_name lobby_team_head" :placeholder="place_holder" @blur="save_team_name">
    <input v-if="team_number == 'team_2'" id="lobby_team_head_2" maxlength="10" class="team_name lobby_team_head" :placeholder="place_holder" @blur="save_team_name">
    <input v-if="team_number == 'team_3'" id="lobby_team_head_3" maxlength="10" class="team_name lobby_team_head" :placeholder="place_holder" @blur="save_team_name">
    <input v-if="team_number == 'team_4'" id="lobby_team_head_4" maxlength="10" class="team_name lobby_team_head" :placeholder="place_holder" @blur="save_team_name">
    <hr>
    <div v-if="view == 'menu'" style="width:100%;height:80%;margin-top:15%;">
        <div class="button_menu" @click="view = 'team'">Состав команды ({{team.length}})</div>
        <div class="button_menu" @click="view = 'logo'">Лого команды</div>
        <img v-if="selected_logo != null" :src="logos[selected_logo]" style="width:40%;height:40%;float:left;margin-left:30%;margin-top:5%;">
    </div>
    <div v-if="view == 'team'" style="width:100%;height:100%;">
        <div class="scroll">
            <div class="one_player" v-for="(option, index) in team" :key="index" :index="index">
                <input :id="team_number + '_player_' + index" type="text" :placeholder="'Новый игрок'" :value="option" class="wait_users" @blur="save_edit">
                <div id="delete_player" @click="team_pop">x</div>
            </div>
            <div id="plus_player" @click="add_player(team_number)">+</div>
        </div>
    </div>
    <div v-if="view == 'logo'" style="width:100%;height:100%;">
        <img class="logo_option" v-for="(option, index) in logos" :src="option" :key="index" alt="" style="width:22.5%;height:22.5%;float:left;margin-left:1.6%;" @click="select_logo(index, $event)">
    </div>
</div>
</template>

<script>
import { SRC_PATH } from '../common_const.js'

export default {
  name: 'ManageTeam',
  props:['team', 'team_number', 'place_holder'],
  data(){
    return {
        new_added: false,
        last_added_id: null,
        view: 'menu',
        logos: [SRC_PATH + 'team_logo_1.svg', SRC_PATH + 'team_logo_2.svg',
                SRC_PATH + 'team_logo_3.svg', SRC_PATH + 'team_logo_4.svg'],
        selected_logo: null
    }
  },
  methods: {
        select_logo: function (index, event) {
            var logo_arr = event.target.parentElement.children;
            for( var i = 0; i < logo_arr.length; ++i )
                logo_arr.item(i).style.border = '';
            event.target.style.border = 'solid 0.01vw black';
            let mapping = new Map();
            mapping.set("team_1", 0).set("team_2", 1).set("team_3", 2).set("team_4", 3);
            this.selected_logo = index;
            this.$emit('set-logo', mapping.get(this.team_number), index);
        },
        team_pop: function () {
            this.$emit('team-pop', this.team_number);
        },
        add_player: function () {
            this.new_added = true;
            this.last_added_id = this.team_number + '_player_' + Number(this.team.length);
            this.$emit('add-player', this.team_number);
        },
        save_edit: function (event) {
            this.$emit('save-edit', this.team_number, event.target.parentElement.getAttribute('index'), event.target.value);
        },
        save_team_name: function () {
            let value;
            if( this.team_number == 'team_1' )
                value = document.getElementById('lobby_team_head_1').value;
            else if( this.team_number == 'team_2' )
                value = document.getElementById('lobby_team_head_2').value;
            else if( this.team_number == 'team_3' )
                value = document.getElementById('lobby_team_head_3').value;
            else if( this.team_number == 'team_4' )
                value = document.getElementById('lobby_team_head_4').value;
            this.$emit('save-team-name', this.team_number, value);
        },
  },
  updated: function () {
    if(this.new_added)
    {
        document.getElementById(this.last_added_id).focus();
        this.new_added = false;
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.lobby_team_head{
    margin-top: 2%;
    text-align: center;
    font-size: 1.5vw;
    width: 100%;
}

.lobby_team{
    border-radius: 1.1vw;
    float: left;
    border: solid black 0.15vw;
    width: 45%;
    height: 46%;
    margin-left: 4%;
    background: rgba(255, 255, 255, 0.7);
}

.one_player{
    width: 100%;
}

.wait_users{
    margin-top: 2%;
    margin-left: 10%;
    width: 75%;
    border: solid black 0.15vw;
    border-radius: 10px;
    text-align: center;
    font-size: 1.3vw;
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

.team_name{
    float: left;
    background: rgba(255, 255, 255, 0);
    outline: none;
    border: none;
}

hr{
    width: 90%;
    color: green;
    float: left;
    margin-left: 5%;
}

.scroll{
    float: left;
    height: 80%;
    width: 90%;
    margin-left: 5%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.button_menu{
    text-align: center;
    font-size: 1.5vw;
    margin-top: 5%;
    height: 10%;
    width: 100%;
}
</style>
