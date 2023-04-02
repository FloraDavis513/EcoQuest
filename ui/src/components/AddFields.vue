<template>
    <div id="pop_up" class="pop_up" v-if="is_add_product">
        <div class="pop_up_title">Создание игрового поля</div>
        <input id="pop_up_input" class="pop_up_input" placeholder="Название продукта"/>
        <div class="pop_up_color_title">Выберите цвет поля</div>
        <input id="create_color_switcher" type="color" style="width:35%;height:25%;">
        <div class="pop_up_group_button">
            <div @click="create_product" class="pop_up_button">Сохранить</div>
            <div @click="exit" class="pop_up_button">Выйти</div>
        </div>
    </div>
</template>

<script>
export default {
  name: 'AddFields',
  props:['is_add_product'],
  data(){
    return {
        
        }
    },
    methods:{
        exit: function () {
            this.$emit('close-add-field');
        },
        create_product: function () {
            var name = document.getElementById("pop_up_input").value;
            var color = String(document.getElementById('create_color_switcher').value);
            this.$emit('create-field', name, color);
            this.$emit('close-add-field');
            document.getElementById("pop_up_input").value = "";
        }
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.pop_up{
    float: left;
    position: fixed;
    width: 45%;
    height: 60%;
    background-color: white;
    text-align: center;
    border-radius: 20px;
    border: 2px solid black;
    left: 27.5%;
    top: 27.5%;
}

.pop_up_title{
    margin-top: 2%;
    color: green;
    font-size: 1.8vw;
    font-weight: bold;
}
.pop_up_color_title{
    margin-top: 4%;
    color: green;
    font-size: 1.5vw;
    width: 90%;
    margin-left: 5%;
    text-align: center;
    font-weight: bold;
    margin-bottom: 3%;
}

input{
    width: 60%;
    border-bottom: 1.5px solid silver;
    border-top: none;
    border-left: none;
    border-right: none;
    outline:none;
    font-size: 1.5vw;
}

#pop_up_input{
    margin-top: 5%;
    width: 80%;
}

.pop_up_group_button{
    margin-top: 5%;
    width: 90%;
    height: 20%;
    margin-left: 5%;
}

.pop_up_button{
    float: left;
    margin-left: 5%;
    margin-right: 5%;
    padding-top: 2.5%;
    padding-bottom: 2.5%;
    width: 40%;
    background-color: green;
    color: #ffffff;
    border-radius: 35px;
    font-size: 1.3vw;
    font-weight: bold;
    margin-top: 5%;
}

.pop_up_button:hover {
    box-shadow: 0 0 10px 100px orange inset;
}

</style>
