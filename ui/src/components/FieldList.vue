<template>
    <div class="scroll">
     <div class="grid" id="grid">
        <div class="grid_element" v-for="(option, index) in products" @click="select_product" :key="index" v-bind:style="get_color(option)" :data-db_id="option.id">{{ option.text }}</div>
        <div class="grid_element plus" @click="add_product">+</div>
     </div>
    </div>
</template>

<script>
export default {
  name: 'FieldList',
  props:['products'],
  data(){
    return {
    }
  },
  methods: {
        go_to_fields: function () {
            this.$emit('go-to-fields');
        },
        add_product: function () {
            this.$emit('add-field');
        },
        get_color: function (product) {
            if(product.text == "Финал" || product.text == "Полуфинал")
                return 'color:black;background-color:white;'
            else
                return product.color;
        },
        select_product: function (event) {
            var q = null;
            this.products.forEach(function(item) {
                    if(item.id == event.target.dataset.db_id)
                    {
                        q = item.questions;
                    }
                });
            this.$emit('select-product', event.target.innerHTML, 'background:' + event.target.style.backgroundColor, event.target.dataset.db_id, q);
        },
        save_template_product: function (data) {
            this.db_products = data;
    },
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.grid{
    margin-top: 3%;
    margin-left: 5%;
    width: 90%;
    height: 80%;
}

.grid_element{
    margin-left: 2.6%;
    margin-right: 2%;
    margin-bottom: 5%;
    float: left;
    width: 15%;
    height: 20%;
    border: 0.15vw solid black;
    border-radius: 1vw;
    text-align: center;
    font-size: 220%;
    font-size: 1.8vw;
    line-height: 100px;
    line-height: 6.3vw;
    color: white;
    transition: transform .25s ease;
}

.grid_element:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.number{
    color: black;
    font-size: 2vw;
}

.plus{
    font-size: 6vw;
    border: 2px solid silver;
    color: silver;
}

.scroll{
    height: 86%;
    overflow: auto;
    -ms-overflow-style: none;
    scrollbar-width: none;
    margin-top: 1%;
    width: 100%;
}

.scroll::-webkit-scrollbar {
    width: 0;
    height: 0;
}
</style>
