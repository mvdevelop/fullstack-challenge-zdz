<template>
  <div>
    <!-- Header -->
    <div class="pf-page-header">
      <v-icon icon="mdi-swap-horizontal" size="20" color="primary" />
      <h5>Transações</h5>
    </div>

    <!-- Filtros -->
    <div class="pf-filter-bar">
      <v-text-field
        v-model="filtro.busca"
        placeholder="Buscar transação..."
        prepend-inner-icon="mdi-magnify"
        clearable
        hide-details
        density="compact"
        variant="outlined"
        class="pf-filter-input"
        @update:model-value="aplicarFiltro"
      />
      <v-select
        v-model="filtro.tipo"
        :items="tiposFiltro"
        hide-details
        clearable
        density="compact"
        variant="outlined"
        style="max-width:160px"
        @update:model-value="aplicarFiltro"
      />
      <v-text-field
        v-model="filtroMes"
        label="Mês"
        type="month"
        hide-details
        density="compact"
        variant="outlined"
        style="max-width:160px"
        @update:model-value="aplicarFiltro"
      />
    </div>

    <!-- Botão nova transação -->
    <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirDialog()" class="mb-4">
      Nova transação
    </v-btn>

    <!-- Lista de transações -->
    <div class="pf-card">
      <div v-if="transacoes.length === 0" class="text-center text-medium-emphasis py-6">
        Nenhuma transação encontrada
      </div>
      <div v-for="t in transacoesFiltradas" :key="t.id" class="pf-row">
        <div class="pf-dot" :class="t.tipo === 'Receita' ? 'green' : 'red'">
          <v-icon :icon="t.tipo === 'Receita' ? 'mdi-arrow-down' : 'mdi-arrow-up'" size="16" />
        </div>
        <div class="pf-row-info">
          <div class="pf-row-title">{{ t.descricao }}</div>
          <div class="pf-row-sub">{{ formatarDataCurta(t.data) }} · {{ t.categoria?.nome ?? '—' }}</div>
        </div>
        <div class="d-flex align-center ga-2">
          <span class="pf-badge" :class="t.tipo === 'Receita' ? 'green' : 'red'">
            {{ t.tipo === 'Receita' ? 'Receita' : 'Despesa' }}
          </span>
          <div class="pf-row-val" :class="t.tipo === 'Receita' ? 'text-pf-green' : 'text-pf-red'">
            {{ t.tipo === 'Receita' ? '+' : '-' }} R$ {{ formatarValor(t.valor) }}
          </div>
          <v-btn icon="mdi-pencil" size="x-small" variant="text" @click.stop="abrirDialog(t)" />
          <v-btn icon="mdi-delete" size="x-small" variant="text" color="error" @click.stop="confirmarExclusao(t)" />
        </div>
      </div>
    </div>

    <!-- Dialog criar/editar -->
    <TransacaoDialog
      v-model="dialog"
      :transacao="transacaoSelecionada"
      :contas="contas"
      :categorias="categorias"
      @salvo="onSalvo"
    />

    <!-- Dialog confirmar exclusão -->
    <v-dialog v-model="dialogExclusao" max-width="400">
      <v-card rounded="lg">
        <v-card-title class="pa-6 pb-2">Confirmar exclusão</v-card-title>
        <v-card-text class="pa-6 pt-3">
          Tem certeza que deseja excluir a transação
          <strong>{{ transacaoSelecionada?.descricao }}</strong>?
        </v-card-text>
        <v-card-actions class="pa-6 pt-0">
          <v-spacer />
          <v-btn variant="text" @click="dialogExclusao = false">Cancelar</v-btn>
          <v-btn color="error" variant="flat" :loading="loading" @click="excluir">Excluir</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import type { Transacao, FiltroTransacao } from '~/types'
import { useTransacaoStore } from '~/stores/transacoes'

definePageMeta({ layout: 'default', middleware: 'auth' })

const store = useTransacaoStore()
const { transacoes, loading } = storeToRefs(store)
const { contas, buscar: buscarContas } = useContas()
const { categorias, buscar: buscarCategorias } = useCategorias()

const dialog = ref(false)
const dialogExclusao = ref(false)
const transacaoSelecionada = ref<Transacao | null>(null)

const filtro = reactive<FiltroTransacao>({
  busca: '',
  tipo: undefined,
  categoriaId: undefined,
  dataInicio: undefined,
  dataFim: undefined,
})

const filtroMes = ref(new Date().toISOString().slice(0, 7))

const tiposFiltro = ['Todos', 'Receita', 'Despesa', 'Transferencia']

const transacoesFiltradas = computed(() => transacoes.value)

function formatarValor(v: number) {
  return v.toLocaleString('pt-BR', { minimumFractionDigits: 2 })
}

function formatarDataCurta(d: string) {
  const dt = new Date(d)
  return `${String(dt.getDate()).padStart(2, '0')}/${String(dt.getMonth() + 1).padStart(2, '0')}`
}

function abrirDialog(transacao?: Transacao) {
  transacaoSelecionada.value = transacao ?? null
  dialog.value = true
}

function confirmarExclusao(transacao: Transacao) {
  transacaoSelecionada.value = transacao
  dialogExclusao.value = true
}

async function excluir() {
  if (!transacaoSelecionada.value) return
  await store.excluir(transacaoSelecionada.value.id)
  dialogExclusao.value = false
}

function onSalvo(t: Transacao) {
  store.buscar(filtro)
}

function aplicarFiltro() {
  const mes = filtroMes.value
  if (mes) {
    filtro.dataInicio = `${mes}-01`
    const [ano, m] = mes.split('-')
    const ultimoDia = new Date(parseInt(ano), parseInt(m), 0).getDate()
    filtro.dataFim = `${mes}-${String(ultimoDia).padStart(2, '0')}`
  } else {
    filtro.dataInicio = undefined
    filtro.dataFim = undefined
  }
  store.buscar({
    ...filtro,
    tipo: filtro.tipo === 'Todos' ? undefined : filtro.tipo,
  })
}

onMounted(async () => {
  aplicarFiltro()
  await Promise.all([
    buscarContas(),
    buscarCategorias(),
  ])
})
</script>
