<template>
  <div>
    <div class="d-flex justify-end mb-4">
      <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirDialog()">Nova categoria</v-btn>
    </div>

    <v-row>
      <v-col v-for="cat in categorias" :key="cat.id" cols="6" sm="4" md="3">
        <v-card rounded="lg" class="text-center pa-2">
          <v-card-text>
            <v-avatar :color="cat.cor" size="48" class="mb-2">
              <v-icon :icon="cat.icone" size="24" color="white" />
            </v-avatar>
            <div class="font-weight-medium text-body-2">{{ cat.nome }}</div>
            <v-chip :color="cat.tipo === 'Receita' ? 'success' : cat.tipo === 'Despesa' ? 'error' : 'info'" size="x-small" variant="tonal" class="mt-1">
              {{ cat.tipo }}
            </v-chip>
          </v-card-text>
          <v-card-actions class="justify-center pt-0">
            <v-btn size="x-small" variant="text" icon="mdi-pencil" @click="abrirDialog(cat)" />
            <v-btn size="x-small" variant="text" icon="mdi-delete" color="error" @click="excluir(cat.id)" />
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <v-dialog v-model="dialog" max-width="400" persistent>
      <v-card rounded="lg">
        <v-card-title class="pa-6 pb-2">{{ editando ? 'Editar categoria' : 'Nova categoria' }}</v-card-title>
        <v-card-text class="pa-6 pt-3">
          <v-text-field v-model="form.nome" label="Nome" class="mb-3" />
          <v-select v-model="form.tipo" label="Tipo" :items="['Receita', 'Despesa', 'Ambos']" class="mb-3" />
          <v-text-field v-model="form.icone" label="Ícone MDI" placeholder="mdi-tag" class="mb-3">
            <template #prepend-inner>
              <v-icon :icon="form.icone || 'mdi-tag'" />
            </template>
          </v-text-field>
          <div class="text-caption text-medium-emphasis mb-2">Cor</div>
          <div class="d-flex gap-2 flex-wrap">
            <v-btn
              v-for="cor in coresPredefinidas"
              :key="cor"
              :color="cor"
              size="small"
              icon
              :variant="form.cor === cor ? 'flat' : 'tonal'"
              @click="form.cor = cor"
            >
              <v-icon v-if="form.cor === cor" icon="mdi-check" />
            </v-btn>
          </div>
        </v-card-text>
        <v-card-actions class="pa-6 pt-0">
          <v-spacer />
          <v-btn variant="text" @click="dialog = false">Cancelar</v-btn>
          <v-btn color="primary" variant="flat" :loading="loading" @click="salvar">
            {{ editando ? 'Salvar' : 'Criar' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import type { Categoria, CategoriaForm } from '~/types'

definePageMeta({ layout: 'default', middleware: 'auth' })

const { categorias, loading, buscar, criar, atualizar, excluir } = useCategorias()

const dialog = ref(false)
const editando = ref<Categoria | null>(null)
const form = reactive<CategoriaForm>({ nome: '', tipo: 'Despesa', cor: '#1976D2', icone: 'mdi-tag' })

const coresPredefinidas = ['#1976D2','#2E7D32','#C62828','#F57F17','#6A1B9A','#00838F','#37474F','#E91E63']

function abrirDialog(cat?: Categoria) {
  editando.value = cat ?? null
  if (cat) Object.assign(form, { nome: cat.nome, tipo: cat.tipo, cor: cat.cor, icone: cat.icone })
  else Object.assign(form, { nome: '', tipo: 'Despesa', cor: '#1976D2', icone: 'mdi-tag' })
  dialog.value = true
}

async function salvar() {
  if (editando.value) await atualizar(editando.value.id, form)
  else await criar(form)
  dialog.value = false
}

onMounted(buscar)
</script>
