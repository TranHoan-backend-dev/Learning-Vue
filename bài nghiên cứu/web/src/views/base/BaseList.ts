import { defineComponent } from 'vue';

export default defineComponent({
    data() {
        return {
            store: {
                data: [] as any[],
                total: 0
            }
        }
    },
    methods: {
        getStore() {
            const me = this;
            console.log(me.store)
        },
    },
    mounted() {
        this.getStore()
    }
})