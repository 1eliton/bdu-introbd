from collections import OrderedDict
import os

# TODO:
# remover preposicoes (lib existente)
# remover outrs caracteres nao desejados
# acertar a remocao do disclaimer

# classe
class do_all_stuff:

    unwanted_chars = '.,-_ ;!?\\:#*"'
    wordfrequency = OrderedDict()
    basepath = 'c:\\TempDBClass\\'
    books_to_read_til_end = 5

    # marca a frequencia das palavras
    def mark_words_freq(self):
        books_count = 0
        # le todos os arquivos na pasta
        for filename in os.listdir(do_all_stuff.basepath):
            try:
                books_count += 1
                # abre os arquivos do diretorio - um por vez
                with open(file = (do_all_stuff.basepath + filename), mode= 'r', encoding= 'utf-8') as currentfile:
                    print('Lendo livro ' + filename)
                    text =  currentfile.read()
                    # busca a qtd de posicoes que vai ler - retira o disclaimer
                    limiters = do_all_stuff.define_len_size(self, text = text)
                    # da um substring
                    if (limiters[0] > 0):
                        text = text[limiters[0]:limiters[1]]
                    # splita o texto
                    words_to_read = text.split()
                    # marca a frequencia das palavras
                    for word in words_to_read:
                        pure_word = word.lower().strip(do_all_stuff.unwanted_chars)
                        if pure_word not in do_all_stuff.wordfrequency:
                            do_all_stuff.wordfrequency[pure_word] = 0 
                        do_all_stuff.wordfrequency[pure_word] += 1

                    # para a execução de alguns livros - para testes
                    if books_count >= do_all_stuff.books_to_read_til_end:
                        break

            except:
                continue

        # imprime as frequencias
        # for word, freq in do_all_stuff.wordfrequency.items():
        #     print (word, '-> ', freq)

        # grava o arquivo com as palavras e seus respectivos numeros
        file = open((do_all_stuff.basepath + 'all-words.txt'), mode = 'w', encoding = 'utf-8')
        file.flush()
        counter = 0
        for word, freq in do_all_stuff.wordfrequency.items():
            counter += 1
            thatsallfolks = (str(counter) + ' ["' + word + '", ' + str(freq) + ']\n')
            file.write(thatsallfolks)
        file.close()

    # define tamanho da string que vai ser lida
    def define_len_size(self, text):
        str_begin_pos = "START OF THIS PROJECT"
        str_end_pos = "END OF THIS PROJECT"

        begin_position = text.index(str_begin_pos) if str_begin_pos in text else 0
        end_position = text.index(str_end_pos) if str_end_pos in text else 0

        # if (begin_position > 0 and end_position > 0):
        #     currentfile.seek(begin_position, 0)
        #     for line in currentfile.readlines():
        #         print(line)
        #     exit

        return begin_position, (begin_position + (end_position - begin_position))

# chamada dos metodos
stuffs = do_all_stuff()
stuffs.mark_words_freq()