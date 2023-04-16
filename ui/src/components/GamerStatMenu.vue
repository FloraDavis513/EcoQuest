<template>
     <div id="menu" class="menu">
        <div class="button" @click="edit" id="edit">
            Случайные вопросы
        </div>
        <div class="button" @click="save_product" id="save_product">
            По выбранным темам
        </div>
        <div class="button" @click="delete_button" id="delete">
            Соревнование
        </div>
        <div class="button" @click="back">
            В меню
        </div>
    </div>
</template>

<script>
// import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'GamerStatMenu',
  data(){
    return {
    }
  },
  methods: {
        back: function () {
            if( this.round == 1 )
                this.$emit('fields-1');
            else if( this.round == 2 )
                this.$emit('fields-2');
            else if( this.round == 3 )
                this.$emit('fields-3');
        },
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.menu{
    font-size: 1.5vmax;
    float: left;
    width: 20%;
    text-align: center;
    height: 88%;
    border-right: 0.15vw solid silver;
    margin-right: 1%;
    margin-top: 0.3%;
}

.button{
    margin-left: 5%;;
    max-width: 90%;
    width: 20vmax;
    margin-top: 1vmax;
    background-color: green;
    color: #ffffff;
    font-weight: bold;
    border-radius: 1vmax;
    padding-top: 1vmax;
    padding-bottom: 1vmax;
    text-align: center;
}

.button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>
