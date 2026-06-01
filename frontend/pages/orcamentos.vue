<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-4">
      <div class="d-flex align-center gap-3">
        <v-btn icon="mdi-chevron-left" variant="text" @click="mudarMes(-1)" />
        <span class="text-h6 font-weight-medium">{{ mesFormatado }}</span>
        <v-btn icon="mdi-chevron-right" variant="text" @click="mudarMes(1)" />
      </div>
      <v-btn color="primary" prepend-icon="mdi-plus" @click="dialog = true">
        Novo orçamento
      </v-btn>
    </div>

    <!-- Cards de orçamento -->
    <v-row v-if="!loading && orcamentos.length > 0">
      <v-col v-for="orc in orcamentos" :key="orc.id" cols="12" sm="6" md="4">
        <v-card rounded="lg">
          <v-card-text>
            <div class="d-flex align-center justify-space-between mb-3">
              <div class="d-flex align-center gap-2">
                <v-avatar :color="orc.categoria?.cor || 'primary'" size="36" variant="tonal">
                  <v-icon :icon="orc.categoria?.icone || 'mdi-tag'" size="18" />
                </v-avatar>
                <span class="font-weight-medium">{{ orc.categoria?.nome }}</span>
              </div>
              <v-btn icon="mdi-delete" size="x-small" variant="text" color="error" @click="excluir(orc.id)" />
            </div>

            <div class="d-flex justify-space-between text-body-2 mb-2">
              <span class="text-medium-emphasis">Gasto</span>
              <span class="font-weight-medium">
                {{ formatarValor(orc.valorGasto) }}
                <span class="text-medium-emphasis">/ {{ formatarValor(orc.valorLimite) }}</span>
              </span>
            </div>

            <v-progress-linear
              :model-value="orc.percentualUsado"
              :color="corProgresso(orc.percentualUsado)"
              bg-color="surface-variant"
              rounded
              height="8"
              class="mb-2"
            />

            <div class="d-flex justify-space-between text-caption text-medium-emphasis">
              <span>{{ Math.round(orc.percentualUsado) }}% utilizado</span>
              <span :class="orc.percentualUsado >= 100 ? 'text-error' : 'text-success'">
                {{ orc.percentualUsado >= 100 ? 'Excedeu' : 'Disponível' }}
                {{ formatarValor(Math.abs(orc.valorLimite - orc.valorGasto)) }}
              </span>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-card v-else-if="!loading" rounded="lg" class="py-10 text-center">
      <v-icon icon="mdi-target" size="48" class="mb-3 text-medium-emphasis" />
      <p class="text-medium-emphasis">Nenhum orçamento para este mês</p>
      <v-btn color="primary" variant="tonal" class="mt-3" @click="dialog = true">
        Criar primeiro orçamento
      </v-btn>
    </v-card>

    <v-progress-circular v-else indeterminate color="primary" class="d-block mx-auto mt-10" />

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

function corProgresso(p: number) {
  if (p >= 100) return 'error'
  if (p >= 80) return 'warning'
  return 'success'
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
