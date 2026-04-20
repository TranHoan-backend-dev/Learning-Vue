<script setup lang="ts">
interface Props {
  width?: string;
  height?: string;
  borderRadius?: string;
}

withDefaults(defineProps<Props>(), {
  width: '100%',
  height: '20px',
  borderRadius: '8px'
});
</script>

<template>
  <div 
    class="ms-skeleton" 
    :style="{ width, height, borderRadius }"
  >
    <div class="shimmer"></div>
  </div>
</template>

<style scoped>
.ms-skeleton {
  position: relative;
  overflow: hidden;
  background-color: #e4e4e7; /* Màu nền xám (zinc-200) chuẩn HeroUI */
}

.shimmer {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    rgba(255, 255, 255, 0) 0%,
    rgba(255, 255, 255, 0.4) 50%,
    rgba(255, 255, 255, 0) 100%
  );
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(100%);
  }
}
</style>
