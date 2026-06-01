<template>
  <div>
    <!-- Header -->
    <div class="pf-page-header">
      <v-icon icon="mdi-target" size="20" color="error" />
      <h5>Orçamentos</h5>
      <v-btn icon="mdi-chevron-left" size="small" variant="text" @click="mudarMes(-1)" />
      <span class="text-medium-emphasis" style="font-size:.85rem">{{ mesFormatado }}</span>
      <v-btn icon="mdi-chevron-right" size="small" variant="text" @click="mudarMes(1)" />
    </div>

    <!-- Loading -->
    <div v-if="loading" class="d-flex justify-center py-10">
      <v-progress-circular indeterminate color="primary" />
    </div>

    <!-- Grid de orçamentos -->
    <v-row v-else>
      <v-col v-for="orc in orcamentos" :key="orc.id" cols="12" sm="6">
        <div class="pf-card budget-card">
          <div class="d-flex align-center ga-3 mb-3">
            <div class="pf-budget-icon" :class="corIcone(orc.percentualUsado)">
              <v-icon :icon="orc.categoria?.icone || 'mdi-tag'" size="18" />
            </div>
            <div>
              <div class="font-weight-medium" style="font-size:.85rem">{{ orc.categoria?.nome }}</div>
              <div class="text-caption text-medium-emphasis">{{ mesFormatado }}</div>
            </div>
          </div>

          <div class="d-flex justify-space-between mb-2" style="font-size:.78rem">
            <span class="text-medium-emphasis">{{ formatarValor(orc.valorGasto) }} / {{ formatarValor(orc.valorLimite) }}</span>
            <span class="font-weight-semibold" :class="corPercentual(orc.percentualUsado)">{{ Math.round(orc.percentualUsado) }}%</span>
          </div>

          <div class="pf-progress mb-2">
            <div class="pf-progress-fill" :style="{ width: Math.min(orc.percentualUsado, 100) + '%', background: corBarra(orc.percentualUsado) }" />
          </div>

          <div class="text-caption text-medium-emphasis">
            Faltam {{ formatarValor(Math.max(orc.valorLimite - orc.valorGasto, 0)) }}
          </div>
        </div>
      </v-col>

      <!-- Card "Novo orçamento" -->
      <v-col cols="12" sm="6">
        <div class="pf-budget-new" @click="dialog = true">
          <v-icon icon="mdi-plus" size="24" class="text-medium-emphasis" />
          <span class="text-medium-emphasis font-weight-medium">Novo orçamento</span>
        </div>
      </v-col>

      <!-- Estado vazio -->
      <v-col v-if="orcamentos.length === 0" cols="12" class="text-center text-medium-emphasis py-6">
        Nenhum orçamento para este mês. Crie o primeiro!
      </v-col>
    </v-row>

    <!-- Dialog novo orçamento -->
    <v-dialog v-model="dialog" max-width="440" persistent>
      <v-card rounded="lg">
        <v-card-title class="pa-6 pb-2">Novo orçamento</v-card-title>
        <v-card-text class="pa-6 pt-3">
          <v-select
            v-model="form.categoriaId"
            label="Categoria"
            :items="categorias"
            item-title="nome"
            item-value="id"
            class="mb-3"
          />
          <v-select
            v-model="form.contaId"
            label="Conta"
            :items="contas"
            item-title="nome"
            item-value="id"
            class="mb-3"
          />
          <v-text-field
            v-model.number="form.valorLimite"
            label="Limite mensal"
            prefix="R$"
            type="number"
            min="0"
          />
        </v-card-text>
        <v-card-actions class="pa-6 pt-0">
          <v-spacer />
          <v-btn variant="text" @click="dialog = false">Cancelar</v-btn>
          <v-btn color="primary" variant="flat" :loading="salvando" @click="criar">Criar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import type { Orcamento, OrcamentoForm } from '~/types'

definePageMeta({ layout: 'default', middleware: 'auth' })

const { get, post, del } = useApi()
const { contas, buscar: buscarContas } = useContas()
const { categorias, buscar: buscarCategorias } = useCategorias()

const orcamentos = ref<Orcamento[]>([])
const loading = ref(false)
const salvando = ref(false)
const dialog = ref(false)

const mesRef = ref(new Date())

const mesFormatado = computed(() =>
  mesRef.value.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })
)

const mesISO = computed(() => mesRef.value.toISOString().slice(0, 7))

const form = reactive<OrcamentoForm>({
  categoriaId: '',
  contaId: '',
  valorLimite: 0,
  mesReferencia: mesISO.value,
})

function mudarMes(delta: number) {
  const d = new Date(mesRef.value)
  d.setMonth(d.getMonth() + delta)
  mesRef.value = d
  buscar()
}

function corIcone(p: number) {
  if (p >= 100) return 'red'
  if (p >= 80) return 'amber'
  return 'green'
}

function corPercentual(p: number) {
  if (p >= 100) return 'text-pf-red'
  if (p >= 80) return 'text-pf-amber'
  return 'text-pf-green'
}

function corBarra(p: number) {
  if (p >= 100) return 'var(--pf-red)'
  if (p >= 80) return 'var(--pf-amber)'
  return 'var(--pf-green)'
}

function formatarValor(v: number) {
  return `R$ ${v.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
}

async function buscar() {
  loading.value = true
  try {
    orcamentos.value = await get<Orcamento[]>('/orcamentos', { mes: mesISO.value })
  } finally {
    loading.value = false
  }
}

async function criar() {
  salvando.value = true
  try {
    form.mesReferencia = mesISO.value
    const novo = await post<Orcamento>('/orcamentos', form)
    orcamentos.value.push(novo)
    dialog.value = false
  } finally {
    salvando.value = false
  }
}

async function excluir(id: string) {
  await del(`/orcamentos/${id}`)
  orcamentos.value = orcamentos.value.filter(o => o.id !== id)
}

onMounted(async () => {
  await Promise.all([buscar(), buscarContas(), buscarCategorias()])
})
</script>
