<template>
<div id="lobby_header">Лобби</div>
<div id="group_lobby">
    <div id="player_list">
        <div id="player_list_head">Список игроков</div>
        <hr>
        <!-- <div v-for="(option, index) in users" :key="index" class="wait_users">
            {{option}}
        </div> -->
    </div>
    
    <div id="group_teams">
        <ManageTeam :team="team_1" :team_number="'team_1'" :place_holder="'Команда 1'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name"  @set-logo="set_logo" />
        <ManageTeam :team="team_2" :team_number="'team_2'" :place_holder="'Команда 2'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name"  @set-logo="set_logo" />
        <ManageTeam :style="team_4.length > 0 ? 'margin-top:5%;' : 'margin-top:5%;visibility:hidden;'" :team="team_3" :team_number="'team_3'" :place_holder="'Команда 4'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name"  @set-logo="set_logo" />
        <ManageTeam :style="team_2.length > 0 ? 'margin-top:5%;' : 'margin-top:5%;visibility:hidden;'" :team="team_4" :team_number="'team_4'" :place_holder="'Команда 3'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name" @set-logo="set_logo" />
    </div>
    <div id="player_list">
        <div id="player_list_head">Настройки комнаты</div>
        <hr>
        <div id="room_id_header">ID</div>
        <div id="room_id">00000</div>
        <div id="global_timer_header">Таймер</div>
        <div id="global_timer">
            <input id="timer_hours" maxlength="2" class="clock" type="text" placeholder="00"> 
            <div class="clock_dil">:</div>
            <input id="timer_min" maxlength="2" class="clock" type="text" placeholder="00"> 
            <div class="clock_dil">:</div>
            <input id="timer_sec" maxlength="2" class="clock" type="text" placeholder="00">
        </div>
        <div id="critical_timer_header">Критическая отметка</div>
        <div id="critical_timer">
            <input id="crit_timer_hours" maxlength="2" class="clock" type="text" placeholder="00"> 
            <div class="clock_dil">:</div>
            <input id="crit_timer_min" maxlength="2" class="clock" type="text" placeholder="00"> 
            <div class="clock_dil">:</div>
            <input id="crit_timer_sec" maxlength="2" class="clock" type="text" placeholder="00">
        </div>
        <div id="start_game" @click="start_game">Начать игру</div>
    </div>
</div>
</template>

<script>
import ManageTeam from './components/ManageTeam.vue'
import { SERVER_PATH } from './common_const.js'

export default {
  name: 'GameLobby',
  components: {
    ManageTeam
  }, 
  data(){
    return {
        team_1: [
        ],
        team_2: [
        ],
        team_3: [
        ],
        team_4: [
        ],
        last_added_id: null,
        new_added: false,
        teams_name: ['','','',''],
        team_logos: [null, null, null, null]
    }
  },
  methods: {
        set_logo: function (index, logo) {
            this.team_logos[index] = logo;
        },
        team_pop: function (team) {
            if( team == 'team_1' )
                this.team_1.pop();
            else if( team == 'team_2' )
                this.team_2.pop();
            else if( team == 'team_3' )
                this.team_3.pop();
            else if( team == 'team_4' )
                this.team_4.pop();
        },
        add_player: function (team) {
            if( team == 'team_1' )
                this.team_1.push('');
            else if( team == 'team_2' )
                this.team_2.push('');
            else if( team == 'team_3' )
                this.team_3.push('');
            else if( team == 'team_4' )
                this.team_4.push('');
            this.new_added = true;
            
        },
        save_edit: function (team, index, value) {
            if(value == '')
                return;

            if( team == 'team_1' )
                this.team_1[index] = value;
            else if( team == 'team_2' )
                this.team_2[index] = value;
            else if( team == 'team_3' )
                this.team_3[index] = value;
            else if( team == 'team_4' )
                this.team_4[index] = value;
        },
        save_team_name: function (team, value) {
            if(value == '')
                return;

            if( team == 'team_1' )
                this.teams_name[0] = value;
            else if( team == 'team_2' )
                this.teams_name[1] = value;
            else if( team == 'team_3' )
                this.teams_name[2] = value;
            else if( team == 'team_4' )
                this.teams_name[3] = value;
        },
        start_game: function () {
            this.sleep(300);
            for( var i = 0; i < this.teams_name.length; ++i )
            {
                if(this.teams_name[i] == '')
                    this.teams_name[i] = 'Команда ' + String( i + 1 );
            }
            const toFindDuplicates = arry => arry.filter((item, index) => arry.indexOf(item) !== index && item !== null);
            const duplicateElements = toFindDuplicates(this.team_logos);
            if(duplicateElements.length > 0)
            {
                alert( "Присутствуют повторяющиеся логотипы команд" );
                return;
            }
            this.$emit('start-game', this.team_logos, this.teams_name, [this.team_1, this.team_2, this.team_3, this.team_4], 
            [document.getElementById('timer_hours').value.length == 0 ? '00' : document.getElementById('timer_hours').value, document.getElementById('timer_min').value.length == 0 ? '00' : document.getElementById('timer_min').value, document.getElementById('timer_sec').value.length == 0 ? '00' : document.getElementById('timer_sec').value], 
            [document.getElementById('crit_timer_hours').value.lenght == 0 ? '00' : document.getElementById('crit_timer_hours').value, document.getElementById('crit_timer_min').value.lenght == 0 ? '00' : document.getElementById('crit_timer_min').value, document.getElementById('crit_timer_sec').value.lenght == 0 ? '00' : document.getElementById('crit_timer_sec').value]);
        },
        sleep: function (milliseconds) {
            var start = new Date().getTime();
            for (var i = 0; i < 1e7; i++) {
                if ((new Date().getTime() - start) > milliseconds){
                break;
                }
            }
        },
  },
  mounted: function () {
      fetch(SERVER_PATH + "/game/chooseQuestion", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({questionId:1, questionType:"REGULAR", state:"game_begin"})
                });
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#lobby_header{
    text-align: center;
    font-size: 2vw;
}

#group_lobby{
    width: 100%;
    height: 98%;
    background: #21A038;
}

#player_list{
    margin-top: 1%;
    margin-left: 2%;
    width: 17.5%;
    height: 95%;
    border: solid black 2px;
    border-radius: 20px;
    float: left;
    background: rgba(255, 255, 255, 0.7);
}

#player_list_head{
    margin-top: 2%;
    text-align: center;
    font-size: 1.5vw;
}

hr{
    width: 90%;
    color: green;
}

#group_teams{
    margin-left: 1%;
    margin-top: 1%;
    float: left;
    width: 57%;
    height: 95%;
}

#room_id_header{
    margin-top: 5%;
    text-align: center;
    font-size: 1.75vw;
    text-decoration: underline;
}

#room_id{
    margin-top: 2%;
    text-align: center;
    font-size: 0.9vw;
}

#global_timer_header{
    text-align: center;
    margin-top: 5%;
    font-size: 1.75vw;
    text-decoration: underline;
}

#global_timer{
    width: 80%;
    text-align: center;
    margin-top: 1%;
    font-size: 3vw;
    height: 10%;
    margin-left: 13%;
}

#critical_timer_header{
    text-align: center;
    margin-top: 5%;
    font-size: 1.6vw;
    text-decoration: underline;
}

#critical_timer{
    width: 80%;
    text-align: center;
    margin-top: 1%;
    font-size: 3vw;
    height: 10%;
    margin-left: 13%;
}

#start_game{
    text-align: center;
    width: 80%;
    height: 5%;
    margin-left: 10%;
    border: solid black 2px;
    border-radius: 15px;
    margin-top: 100%;
    font-size: 1.3vw;
    background: green;
    color: white;
    font-weight: bold;
    line-height: 150%;
}

#start_game:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

.wait_users{
    margin-top: 2%;
    margin-left: 10%;
    width: 75%;
    border: solid black 2px;
    border-radius: 10px;
    text-align: center;
    font-size: 1.3vw;
    padding-top: 1.5%;
    padding-bottom: 1.5%;
    float: left;
}

.scroll{
    height: 80%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.clock{
    font-size: 100%;
    width: 25%;
    float: left;
    background: rgba(255, 255, 255, 0);
    outline: none;
    border: none;
}

.clock_dil{
    float: left;
}
</style>