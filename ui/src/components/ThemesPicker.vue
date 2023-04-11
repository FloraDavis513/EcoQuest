<template>
    <div id="picker">
        <div id="menu_header">Выберите продукты для игры</div>
        <div id="group_themes">
            <div v-for="(option, index) in products" :key="index" class="group_block name_product" @click="select_product($event, option)">
                {{option.name}}
            </div>
        </div>
        <div id="group_button">
            <div class="button" @click="to_mode_switcher">Назад</div>
            <div class="button" @click="start_quiz">Начать</div>
        </div>
    </div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'ThemesPicker',
  data(){
    return {
        products: [],
        selected_product: new Set()
    }
  },
  methods:{
    fill_product_list: function (products) {
        this.products = products;
    },
    select_product: function (event, option) {
        if(this.selected_product.has(option.productId))
        {
            event.target.style.backgroundColor = 'white';
            this.selected_product.delete(option.productId);
            return;
        }
        event.target.style.backgroundColor = 'lime';
        this.selected_product.add(option.productId);
    },
    to_mode_switcher: function () {
        this.$emit('switch-mode');
    },
    start_quiz: function () {
        this.$emit('start-quiz', Array.from(this.selected_product));
    },
  },
  beforeMount: async function () {
        fetch(SERVER_PATH + "/product/get/all/3", {
            method: "GET",
            headers: {'Content-Type': 'application/json'}
            }).then( res => res.json() ).then( data => this.fill_product_list(data) );
  },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#picker{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    font-size: 1.9vmax;
}

#group_button{
    margin-top: 3vmax;
    width: 100%;
    display: flex;
    justify-content: space-around;
    text-align: center;
    font-size: 1.9vmax;
}

#menu_header{
    margin-top: 1vmax;
}

#group_themes{
    font-size: 1.9vmax;
    margin-left: 2vmax;
}

.group_block{
    float: left;
    display: flex;
    align-items: center;
    text-align: center;
    border: black solid 0.2vmax;
    border-radius: 0.75vmax;
    padding: 1vmax;
    margin-right: 2vmax;
    margin-top: 3vmax;
}

.checkbox_product{
    width: 1.5vmax;
    height: 1.5vmax;
    flex-grow: 1;
}

.name_product{
    flex-grow: 4;
}

.button{
    width: 25vmax;
    margin-top: 1%;
    background-color: green;
    color: #ffffff;
    font-weight: bold;
    border-radius: 1.5vmax;
    padding-top: 1vmax;
    padding-bottom: 1vmax;
    text-align: center;
}

.button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>