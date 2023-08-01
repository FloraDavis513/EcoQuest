<template>
     <div id="menu" class="menu">
        <div id="avatar" class="avatar" :style="selected_product.colour">
            {{selected_product.name}}
        </div>

        <div @click="edit" id="edit">
            Редактировать
        </div>
        <!-- <div @click="save_product" id="save_product">
            Сохранить
        </div> -->
        <div @click="delete_button" id="delete">
            Удалить
        </div>
        <div @click="back" id="back">
            К другим полям
        </div>
    </div>
</template>

<script>
import { SERVER_PATH } from '../common_const.js'

export default {
  name: 'ProductMenu',
  props:['selected_product', 'current_view', 'round'],
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
        edit: function () {
            this.$emit('edit-product');
        },
        delete_button: function () {
            this.$emit('delete-product');
        },
        sleep: function (milliseconds) {
            var start = new Date().getTime();
            for (var i = 0; i < 1e7; i++) {
                if ((new Date().getTime() - start) > milliseconds){
                break;
                }
            }
        },
        save_product: function () {
            if(this.selected_product.productId)
            {
                fetch(SERVER_PATH + "/product/update", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({id:this.selected_product[2], name: this.selected_product[0], colour: this.selected_product[1], questions:this.selected_product[3]})
                })
            }
            else
            {
                fetch(SERVER_PATH + "/product/create", {
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({name: this.selected_product[0], colour: this.selected_product[1], questions:this.selected_product[3]})
                })
            }
            this.sleep(300);
            this.$emit('to-fields');
        },
  },
  mounted: function () {
  this.$nextTick(function () {
    // Код, который будет запущен только после
    // отображения всех представлений
    if(this.selected_product[0] == 'Финал' || this.selected_product[0] == 'Полуфинал')
    {
        document.getElementById('edit').style.visibility = 'hidden';
        document.getElementById('delete').style.visibility = 'hidden';
    }
  })
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.menu{
    float: left;
    width: 20%;
    text-align: center;
    height: 88%;
    border-right: 0.15vw solid silver;
    margin-right: 1%;
    margin-top: 0.3%;
}

.avatar{
    margin-left: 15%;
    width: 70%;
    padding-top: 15%;
    padding-bottom: 15%;
    border-radius: 20px;
    border: solid black 2px;
    font-size: 1.5vw;
    margin-top: 5%;
}

#edit, #back, #delete, #save_product{
    border-radius: 20px;
    padding-top: 4%;
    padding-bottom: 4%;
    margin-left: 10%;
    width: 80%;
    color: white;
    background-color: green;
    font-size: 1.1vw;
    font-weight: bold;
}

#edit{
    margin-top: 10%;
}

#delete{
    margin-top: 5%;
}

#back{
    margin-top: 80%;
}

#save_product{
    margin-top: 5%;
}

#edit:hover, #back:hover, #delete:hover, #save_product:hover {
    box-shadow: 0 0 10px 100px orange inset;
}
</style>
