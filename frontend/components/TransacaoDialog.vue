<template>
  <v-dialog v-model="model" max-width="520" persistent>
    <v-card rounded="lg">
      <v-card-title class="pa-6 pb-2">
        {{ editando ? 'Editar transação' : 'Nova transação' }}
      </v-card-title>

      <v-card-text class="pa-6 pt-4">
        <!-- Tipo -->
        <v-btn-toggle v-model="form.tipo" mandatory divided rounded="lg" class="mb-5 w-100">
          <v-btn value="Receita" color="success" class="flex-1-1">
            <v-icon start icon="mdi-arrow-down-circle" />
            Receita
          </v-btn>
          <v-btn value="Despesa" color="error" class="flex-1-1">
            <v-icon start icon="mdi-arrow-up-circle" />
            Despesa
          </v-btn>
          <v-btn value="Transferencia" color="info" class="flex-1-1">
            <v-icon start icon="mdi-swap-horizontal" />
            Transferência
          </v-btn>
        </v-btn-toggle>

        <v-text-field
          v-model.number="form.valor"
          label="Valor"
          prefix="R$"
          type="number"
          step="0.01"
          min="0"
          :rules="[v => v > 0 || 'Valor deve ser maior que zero']"
          class="mb-3"
        />

        <v-text-field
          v-model="form.descricao"
          label="Descrição"
          :rules="[v => !!v || 'Descrição obrigatória']"
          class="mb-3"
        />

        <v-row>
          <v-col cols="6">
            <v-select
              v-model="form.categoriaId"
              label="Categoria"
              :items="categoriasOpcoes"
              item-title="nome"
              item-value="id"
              :rules="[v => !!v || 'Selecione uma categoria']"
            />
          </v-col>
          <v-col cols="6">
            <v-select
              v-model="form.contaId"
              label="Conta"
              :items="contasOpcoes"
              item-title="nome"
              item-value="id"
              :rules="[v => !!v || 'Selecione uma conta']"
            />
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="6">
            <v-text-field
              v-model="form.data"
              label="Data"
              type="date"
              :rules="[v => !!v || 'Data obrigatória']"
            />
          </v-col>
          <v-col cols="6">
            <v-select
              v-model="form.status"
              label="Status"
              :items="['Confirmada', 'Pendente']"
            />
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="pa-6 pt-0">
        <v-spacer />
        <v-btn variant="text" @click="fechar">Cancelar</v-btn>
        <v-btn
          color="primary"
          variant="flat"
          :loading="salvando"
          @click="salvar"
        >
          {{ editando ? 'Salvar alterações' : 'Criar transação' }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import type { Transacao, TransacaoForm, Conta, Categoria } from '~/types'

const props = defineProps<{
  transacao?: Transacao | null
  contas: Conta[]
  categorias: Categoria[]
}>()

const emit = defineEmits<{
  salvo: [transacao: Transacao]
  fechado: []
}>()

const model = defineModel<boolean>()

const { post, put } = useApi()
const salvando = ref(false)

const editando = computed(() => !!props.transacao)

const formVazio = (): TransacaoForm => ({
  contaId: '',
  categoriaId: '',
  valor: 0,
  tipo: 'Despesa',
  descricao: '',
  data: new Date().toISOString().split('T')[0],
  status: 'Confirmada',
})

const form = reactive<TransacaoForm>(formVazio())

watch(() => props.transacao, (t) => {
  if (t) {
    Object.assign(form, {
      contaId: t.contaId,
      categoriaId: t.categoriaId,
      valor: t.valor,
      tipo: t.tipo,
      descricao: t.descricao,
      data: t.data.split('T')[0],
      status: t.status,
    })
  } else {
    Object.assign(form, formVazio())
  }
}, { immediate: true })

const categoriasOpcoes = computed(() =>
  props.categorias.filter(c => c.tipo === form.tipo || c.tipo === 'Ambos')
)

const contasOpcoes = computed(() => props.contas.filter(c => c.ativa))

async function salvar() {
  salvando.value = true
  try {
    const resultado = editando.value
      ? await put<Transacao>(`/transacoes/${props.transacao!.id}`, form)
      : await post<Transacao>('/transacoes', form)
    emit('salvo', resultado)
    fechar()
  } finally {
    salvando.value = false
  }
}

function fechar() {
  model.value = false
  emit('fechado')
}
</script>
