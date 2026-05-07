<script lang="ts">
import {defineComponent, getCurrentInstance, ref} from "vue";
import BaseList from '../../base/BaseList';
import Candidates from "@/views/ms-candidate/Candidates.vue";

export default defineComponent({
  extends: BaseList,
  name: 'InventoryItemsList',
  components: {
    Candidates
  },
  setup() {
    const instance = getCurrentInstance();
    const proxy = instance?.proxy as any;

    const store = ref({
      data: [],
      total: 0,
      permission: 'inventory-item',
    })

    const handleAdd = () => {
      const me = proxy;
      console.log(me)
    }

    return {
      handleAdd
    }
  },
})
</script>


<template>
<div>
  jjj
</div>
</template>

<style scoped>

</style>