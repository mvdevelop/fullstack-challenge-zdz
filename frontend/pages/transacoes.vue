<template>
  <div>
    <!-- Filtros -->
    <v-card rounded="lg" class="mb-4" variant="flat">
      <v-card-text>
        <v-row align="center">
          <v-col cols="12" sm="4">
            <v-text-field
              v-model="filtro.busca"
              label="Buscar"
              prepend-inner-icon="mdi-magnify"
              clearable
              hide-details
              @update:model-value="aplicarFiltro"
            />
          </v-col>
          <v-col cols="6" sm="2">
            <v-select
              v-model="filtro.tipo"
              label="Tipo"
              :items="['Todos', 'Receita', 'Despesa', 'Transferencia']"
              hide-details
              clearable
              @update:model-value="aplicarFiltro"
            />
          </v-col>
          <v-col cols="6" sm="2">
            <v-select
              v-model="filtro.categoriaId"
              label="Categoria"
              :items="categorias"
              item-title="nome"
              item-value="id"
              hide-details
              clearable
              @update:model-value="aplicarFiltro"
            />
          </v-col>
          <v-col cols="6" sm="2">
            <v-text-field
              v-model="filtro.dataInicio"
              label="De"
              type="date"
              hide-details
              @update:model-value="aplicarFiltro"
            />
          </v-col>
          <v-col cols="6" sm="2">
            <v-text-field
              v-model="filtro.dataFim"
              label="Até"
              type="date"
              hide-details
              @update:model-value="aplicarFiltro"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <!-- Tabela -->
    <v-card rounded="lg">
      <v-card-title class="d-flex align-center justify-space-between pa-5">
        <span class="font-weight-semibold">Transações <v-chip size="small" class="ml-2">{{ transacoes.length }}</v-chip></span>
        <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirDialog()">
          Nova transação
        </v-btn>
      </v-card-title>

      <v-data-table
        :headers="headers"
        :items="transacoes"
        :loading="loading"
        item-value="id"
        hover
      >
        <template #item.tipo="{ item }">
          <v-chip
            :color="item.tipo === 'Receita' ? 'success' : item.tipo === 'Despesa' ? 'error' : 'info'"
            size="small"
            variant="tonal"
          >
            {{ item.tipo }}
          </v-chip>
        </template>

        <template #item.valor="{ item }">
          <span :class="item.tipo === 'Receita' ? 'text-success' : 'text-error'" class="font-weight-semibold">
            {{ item.tipo === 'Receita' ? '+' : '-' }} {{ formatarValor(item.valor) }}
          </span>
        </template>

        <template #item.data="{ item }">
          {{ formatarData(item.data) }}
        </template>

        <template #item.categoria="{ item }">
          <v-chip v-if="item.categoria" size="small" :color="item.categoria.cor" variant="tonal">
            {{ item.categoria.nome }}
          </v-chip>
        </template>

        <template #item.status="{ item }">
          <v-chip
            :color="item.status === 'Confirmada' ? 'success' : item.status === 'Pendente' ? 'warning' : 'error'"
            size="small"
            variant="tonal"
          >
            {{ item.status }}
          </v-chip>
        </template>

        <template #item.actions="{ item }">
          <v-btn icon="mdi-pencil" size="small" variant="text" @click="abrirDialog(item)" />
          <v-btn icon="mdi-delete" size="small" variant="text" color="error" @click="confirmarExclusao(item)" />
        </template>
      </v-data-table>
    </v-card>

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
        <v-card-title>Confirmar exclusão</v-card-title>
        <v-card-text>
          Tem certeza que deseja excluir a transação
          <strong>{{ transacaoSelecionada?.descricao }}</strong>?
        </v-card-text>
        <v-card-actions>
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

const headers = [
  { title: 'Data', key: 'data', sortable: true },
  { title: 'Descrição', key: 'descricao' },
  { title: 'Categoria', key: 'categoria', sortable: false },
  { title: 'Conta', key: 'conta.nome', sortable: false },
  { title: 'Tipo', key: 'tipo' },
  { title: 'Valor', key: 'valor', sortable: true },
  { title: 'Status', key: 'status' },
  { title: '', key: 'actions', sortable: false, align: 'end' },
]

function formatarValor(v: number) {
  return `R$ ${v.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
}

function formatarData(d: string) {
  return new Date(d).toLocaleDateString('pt-BR')
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
  store.buscar({
    ...filtro,
    tipo: filtro.tipo === 'Todos' ? undefined : filtro.tipo,
  })
}

onMounted(async () => {
  await Promise.all([
    store.buscar(),
    buscarContas(),
    buscarCategorias(),
  ])
})
</script>
