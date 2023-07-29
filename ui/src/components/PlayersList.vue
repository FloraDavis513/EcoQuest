<template>
    <div id="masters" class="select_masters">
        <input type="search" placeholder="&#128269; Поиск игрока" class="search_masters" v-model="search_condition"/>
        <div class="scroll">
            <div class="option_masters" v-for="(option, index) in search_options" @click="select(index, $event)" :key="index">
                {{ option.firstName[0] + '. ' + option.patronymic[0] + '. ' + option.lastName }}
            </div>
        </div>
    </div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'PlayersList',
  data(){
    return {
        selected: null,
        counter: 0,
        search_options: [
        ],
        masters: [],
        search_condition: ""
    }
  },
  methods: {
        select: function (index, event) {
            var all_elements = event.target.parentElement.children;
            for (var i = 0; i < all_elements.length; i++) 
            {
                if(i != index)
                    all_elements[i].style.opacity = 1;
                else if(all_elements[i].style.opacity == 0.5)
                    all_elements[i].style.opacity = 1;
                else
                    all_elements[i].style.opacity = 0.5;
            }
            this.selected = this.masters[index];
            this.$emit('master-chosen', true, this.selected);
        },
        unselect: function () {
        var list = document.getElementsByClassName("option_masters");
        for (var i = 0; i < list.length; i++) {
            list[i].style.opacity = 1;
        }
    }
    },
    beforeCreate: function () {
        fetch(SERVER_PATH + "/user/get/activePlayers", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.masters = data );
    },
    created: function () {
            this.counter = this.masters.length;
            this.search_options = this.masters;
        },
    watch: {
        search_condition: function () {
            if(this.search_condition.length == 0)
            {
                this.search_options = this.masters;
                this.search_condition = "";
            }
            else
                this.search_options = this.masters.filter(option => option.lastName.indexOf(this.search_condition) != -1);
        },
        masters: function () {
            this.search_options = this.masters;
        }
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.select_masters{
    float: left;
    width: 20%;
    text-align: center;
    height: 89%;
    border-right: 2px solid silver;
    margin-right: 1%;
}

.scroll{
    height: 90%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}

.master_title{
    font-size: 1.5vw;
    font-weight: bold;
    height: 5%;
}

.search_masters{
    padding-top: 3%;
    padding-bottom: 3%;
    text-align: left;
    border-radius: 35px;
    background-color: green;
    margin-top: 5%;
    color: #ffffff;
    font-size: 1.2vw;
    width: 90%;
    text-indent: 5%;
}

.option_masters{
    padding-top: 3%;
    padding-bottom: 3%;
    text-align: center;
    border-radius: 35px;
    background-color: green;
    margin: 5%;
    color: #ffffff;
    font-size: 1vw;
    font-weight: bold;
}

.option_masters_opacity{
    padding-top: 3%;
    padding-bottom: 3%;
    text-align: center;
    border-radius: 35px;
    background-color: red;
    margin: 5%;
    color: #ffffff;
    font-size: 1vw;
    font-weight: bold;
    
    opacity: 0.5;
}

.option_masters:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>
