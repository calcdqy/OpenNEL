<template>
  <div class="about">
    <div class="card">
      <div class="card-title">版权</div>
      <div class="card-body">
        <div class="text">{{ copyrightText }}</div>
      </div>
    </div>
    <div class="card">
      <div class="card-title">联系方式</div>
      <div class="card-body">
        <div class="text">{{ joinText }}</div>
      </div>
    </div>
    <div class="card">
      <div class="card-title">贡献者</div>
      <div class="card-body">
        <ul class="list">
          <li v-for="(c,i) in contribItems" :key="i" class="item">
            <span v-if="c.role" :class="['tag', c.color]">{{ c.role }}</span>
            <span class="name">{{ c.name }}</span>
          </li>
          <li v-if="contribItems.length === 0" class="empty">暂无贡献者</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import appConfig from '../config/app.js'
const copyrightText = computed(() => appConfig.getAboutCopyright())
const joinText = computed(() => appConfig.getAboutJoin())
const contributors = computed(() => appConfig.getAboutContributors())
const palette = ['tag-blue','tag-green','tag-amber','tag-red','tag-violet','tag-pink','tag-cyan']
const contribItems = computed(() => {
  return contributors.value.map(s => {
    const m = s.match(/^\[(.+?)\]\s*(.*)$/)
    const color = palette[Math.floor(Math.random() * palette.length)]
    if (m) return { role: m[1], name: m[2], color }
    return { role: '', name: s, color }
  })
})
</script>

<style scoped>
.about { display: grid; gap: 16px; width: 66%; margin: 0 auto; align-self: flex-start }
.card { border: 1px solid var(--glass-border); border-radius: 12px; background: var(--glass-surface); color: var(--color-text); backdrop-filter: blur(12px) }
.card-title { display: flex; align-items: center; justify-content: space-between; padding: 12px 16px; font-size: 16px; font-weight: 600; border-bottom: 1px solid var(--color-border) }
.card-body { padding: 12px 16px }
.text { white-space: pre-wrap; font-size: 14px }
.list { list-style: none; margin: 0; padding: 0; display: grid; grid-template-columns: 1fr; gap: 8px }
.item { display: flex; align-items: center; gap: 8px; padding: 8px 10px; border: 1px solid var(--glass-border); border-radius: 8px; background: var(--glass-surface); backdrop-filter: blur(var(--glass-blur)) }
.tag { display: inline-flex; align-items: center; padding: 2px 8px; border-radius: 999px; font-size: 12px; border: 1px solid currentColor; background: transparent }
.tag-blue { color: #3b82f6 }
.tag-green { color: #10b981 }
.tag-amber { color: #f59e0b }
.tag-red { color: #ef4444 }
.tag-violet { color: #8b5cf6 }
.tag-pink { color: #ec4899 }
.tag-cyan { color: #06b6d4 }
.name { font-size: 14px }
.empty { opacity: 0.6 }
</style>