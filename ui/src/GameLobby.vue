<template>
<div id="lobby_header">Лобби</div>
<div id="group_lobby">
    <div id="player_list">
        <div id="player_list_head">Список игроков</div>
        <hr>
        <div v-for="option in players.filter((item) => item.List === 0)" :key="option.title" class="wait_users" draggable="true" @dragstart="startDrag($event, option)">
            {{option.Login}}
        </div>
    </div>
    
    <div id="group_teams">
        <ManageTeam :team_length="team_count(1)" :items="players" :team_number="'team_1'" :place_holder="'Команда 1'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name" @set-logo="set_logo" @start-drag="startDrag" @drop="onDrop($event, 1)"  @dragover.prevent  @dragenter.prevent />
        <ManageTeam :team_length="team_count(2)" :items="players" :team_number="'team_2'" :place_holder="'Команда 2'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name"  @set-logo="set_logo" @start-drag="startDrag" @drop="onDrop($event, 2)"  @dragover.prevent  @dragenter.prevent />
        <ManageTeam :team_length="team_count(3)" :items="players" :style="team_count(4) > 0 ? 'margin-top:5%;' : 'margin-top:5%;visibility:hidden;'" :team_number="'team_3'" :place_holder="'Команда 4'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name"  @set-logo="set_logo" @start-drag="startDrag" @drop="onDrop($event, 3)"  @dragover.prevent  @dragenter.prevent />
        <ManageTeam :team_length="team_count(4)" :items="players" :style="team_count(2) > 0 ? 'margin-top:5%;' : 'margin-top:5%;visibility:hidden;'" :team_number="'team_4'" :place_holder="'Команда 3'" @team-pop="team_pop" @add-player="add_player" @save-edit="save_edit" @save-team-name="save_team_name" @set-logo="set_logo" @start-drag="startDrag" @drop="onDrop($event, 4)"  @dragover.prevent  @dragenter.prevent />
    </div>
    <div id="player_list">
        <div id="player_list_head">Настройки комнаты</div>
        <hr>
        <div id="room_id_header">ID</div>
        <div id="room_id">{{game_id}}</div>
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
  props:['game_id'],
  components: {
    ManageTeam
  }, 
  data(){
    return {
        last_added_id: null,
        new_added: false,
        teams_name: ['','','',''],
        team_logos: [null, null, null, null],
        players: [
      ],
      game: null,
      polling: null
    }
  },
  methods: {
        startDrag(evt, item) {      
            evt.dataTransfer.dropEffect = 'move';      
            evt.dataTransfer.effectAllowed = 'move';      
            evt.dataTransfer.setData('itemID', item.PlayerId);    
        },    
        async onDrop(evt, list) {      
            const itemID = evt.dataTransfer.getData('itemID');      
            const item = this.players.find((item) => item.PlayerId == itemID);     
            item.List = list;
            console.log(item);
            await fetch(SERVER_PATH + '/game/state/players/update/' + this.game_id, {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify(item)
              });
            await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.game = data );
            this.game.state = JSON.parse(this.game.state);
            this.players = this.game.state.Players;
        },
        set_logo: function (index, logo) {
            this.team_logos[index] = logo;
        },
        team_count: function (team_id) {
            return this.players.filter((item) => item.List === team_id).length;
        },
        team_pop: async function (id) {
            // this.players = this.players.filter((item) => item.PlayerId !== id);
            await fetch(SERVER_PATH + '/game/state/players/delete/' + this.game_id + '/' + Number(id), {
                method: 'DELETE',
                headers: {'Content-Type': 'application/json'},
              });
            await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.game = data );
            this.game.state = JSON.parse(this.game.state);
            this.players = this.game.state.Players;
        },
        add_player: async function (team) {
            // this.players.push({name:'', id:this.players.length + 1, list:Number(team.slice(-1))});
            // this.new_added = true;
            await fetch(SERVER_PATH + '/game/state/players/create/' + this.game_id, {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({Login:'', List: Number(team.slice(-1))}),
              });
            await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.game = data );
            this.game.state = JSON.parse(this.game.state);
            this.players = this.game.state.Players;
        },
        save_edit: async function (id, value) {
            if(value == '')
                return;

            let finded_player;

            this.players.forEach((item) => {
                if(item.PlayerId === id)
                    finded_player = item;
            });
            finded_player.Login = value;

            await fetch(SERVER_PATH + '/game/state/players/update/' + this.game_id, {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify(finded_player)
              });
            await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.game = data );
            this.game.state = JSON.parse(this.game.state);
            this.players = this.game.state.Players;
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
        start_game: async function () {
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
            let timer = [document.getElementById('timer_hours').value.length == 0 ? '00' : document.getElementById('timer_hours').value, document.getElementById('timer_min').value.length == 0 ? '00' : document.getElementById('timer_min').value, document.getElementById('timer_sec').value.length == 0 ? '00' : document.getElementById('timer_sec').value];
            let crit_timer = [document.getElementById('crit_timer_hours').value.lenght == 0 ? '00' : document.getElementById('crit_timer_hours').value, document.getElementById('crit_timer_min').value.lenght == 0 ? '00' : document.getElementById('crit_timer_min').value, document.getElementById('crit_timer_sec').value.lenght == 0 ? '00' : document.getElementById('crit_timer_sec').value];
            if( JSON.parse(localStorage.getItem('user')).role != 'player' )
            {
                await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                    method: "GET",
                    headers: {'Content-Type': 'application/json'}
                    }).then( res => res.json() ).then( data => this.game = data );
                this.game.state = JSON.parse(this.game.state);
                this.game.state.teams = [];
                this.game.state.timer = timer;
                this.game.state.crit_timer = crit_timer;
                for(let i = 0; i < this.teams_name.length; ++i)
                {
                    let players_name = [];
                    this.players.filter((item) => item.List === i + 1).forEach(item => players_name.push(item.Login))
                    this.game.state.teams.push({team_name: this.teams_name[i], team_logo: this.team_logos[i], players: players_name});
                }

                await fetch(SERVER_PATH + '/game/update/stateAndQuestion', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    body: JSON.stringify({gameId:this.game_id, state:JSON.stringify(this.game.state), currentQuestionId: null}),
                });
                    
                console.log(this.game.state);
            }
            this.$emit('start-game');
        },
        pollData () {
            this.polling = setInterval(() => {
                fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.game = data );
            this.game.state = JSON.parse(this.game.state);
            this.players = this.game.state.Players;
                }, 5000);
    },
  },
  beforeCreate: async function () {
        await fetch(SERVER_PATH + "/lobby", {
                method: "GET",
                headers: {'Content-Type': 'application/json', "Authorization": "Bearer " + JSON.parse(localStorage.getItem('user')).authorizationToken}
        }).then(res => {
            if(res.status === 401 || res.status === 403)
                this.$emit('logout');
        });
        // await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
        //         method: "GET",
        //         headers: {'Content-Type': 'application/json'}
        //         }).then( res => res.json() ).then( data => this.game = data );
        //     this.game.state = JSON.parse(this.game.state);
        // this.game.state.joined_players.forEach(player => this.players.push({name:player.name, id:this.players.length + 1, list:0}));
    },
    beforeUnmount () {
        clearInterval(this.polling)
    },
    async created () {
        await fetch(SERVER_PATH + "/game/get/" + String(this.game_id), {
                method: "GET",
                headers: {'Content-Type': 'application/json'}
                }).then( res => res.json() ).then( data => this.game = data );
            this.game.state = JSON.parse(this.game.state);
            this.players = this.game.state.Players;
        this.pollData()
    }
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
    font-size: 3vw;
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
    margin-top: 90%;
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
    margin-left: 9%;
    width: 82%;
    border: solid black 2px;
    background: white;
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
    font-size: 3vw;
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