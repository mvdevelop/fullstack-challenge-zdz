<template>
  <v-card :color="color" :variant="variant" rounded="lg">
    <v-card-text>
      <div class="d-flex align-center justify-space-between mb-2">
        <span class="text-caption text-medium-emphasis font-weight-medium text-uppercase">
          {{ label }}
        </span>
        <v-icon :icon="icon" size="20" :color="iconColor" />
      </div>
      <div class="text-h5 font-weight-bold" :class="valueClass">
        {{ formattedValue }}
      </div>
      <div v-if="subtitle" class="text-caption text-medium-emphasis mt-1">
        {{ subtitle }}
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  label: string
  value: number
  icon: string
  iconColor?: string
  color?: string
  variant?: 'flat' | 'outlined' | 'tonal' | 'elevated'
  valueClass?: string
  subtitle?: string
  prefix?: string
}>(), {
  iconColor: 'primary',
  variant: 'flat',
  prefix: 'R$',
})

const formattedValue = computed(() =>
  `${props.prefix} ${props.value.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
)
</script>
