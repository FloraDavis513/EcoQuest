<template>
    <div class="grid" id="grid">
        <div class="grid_element" v-for="(option, index) in template_list" :key="index" :index="index" @click="select_template(index)">{{ option.name }}</div>
        <div class="grid_element plus" @click="add_template">+</div>
    </div>
</template>

<script>
export default {
  name: 'TemplateList',
  props:['template_list'],
  data(){
    return {
    }
  },
  methods: {
        add_template: function () {
            this.$emit('add-template');
        },
        select_template: function (index) {
            this.$emit('select-template', this.template_list[index]);
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
    margin-left: 2%;
    margin-right: 2%;
    margin-bottom: 5%;
    float: left;
    width: 20%;
    height: 20%;
    border: 2px solid black;
    border-radius: 20px;
    text-align: center;
    font-size: 1.7vw;
    line-height: 6.5vw;
    color: black;
    transition: transform .25s ease;
}

.grid_element:hover {
  transform: scale(1.1); /* (150% zoom - Note: if the zoom is too large, it will go outside of the viewport) */
}

.plus{
    font-size: 6vw;
    border: 2px solid silver;
    color: silver;
}
</style>