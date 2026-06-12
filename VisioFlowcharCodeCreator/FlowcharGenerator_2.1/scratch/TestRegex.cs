using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        string code = @"
void prepareText(char* text) {
    size_t j = 0;
    for (size_t i = 0; text[i] != '\0'; i++) {
        char c = to_lower_cp1251(text[i]);
        if (c == 'ё') continue;
        if (getIndex(c) != -1) {
            text[j++] = c;
        }
    }
    text[j] = '\0';
}
void encryptVigenere(char* text, const char* key) {
    char cleanKey[MAX_KEY_LEN + 1];
    prepareKey(key, cleanKey);
    size_t keyLen = strlen(cleanKey);
    if (keyLen == 0) return;

    for (size_t i = 0; text[i] != '\0'; i++) {
        int p = getIndex(text[i]);
        int k = getIndex(cleanKey[i % keyLen]);
        if (p != -1 && k != -1) {
            text[i] = ALPHABET[(p + k) % ALPHABET_SIZE];
        }
    }
}
void decryptVigenere(char* text, const char* key) {
    char cleanKey[MAX_KEY_LEN + 1];
    prepareKey(key, cleanKey);
    size_t keyLen = strlen(cleanKey);
    if (keyLen == 0) return;

    for (size_t i = 0; text[i] != '\0'; i++) {
        int c_idx = getIndex(text[i]);
        int k = getIndex(cleanKey[i % keyLen]);
        if (c_idx != -1 && k != -1) {
            text[i] = ALPHABET[(c_idx - k + ALPHABET_SIZE) % ALPHABET_SIZE];
        }
    }
}
int guessKeyLength(const char* text, int lang) {
    size_t textLen = strlen(text);
    int maxKeyLen = DEFAULT_MAX_KEY_LEN;
    if (maxKeyLen > (int)(textLen / 2)) maxKeyLen = (int)(textLen / 2);
    if (maxKeyLen <= 0) return 1;

    double bestIoC = 0.0;
    int bestLen = 1;

    printf(COLOR_YELLOW ""%s...\n"" COLOR_RESET, (lang == 1) ? ""Подбор длины ключа"" : ""Guessing key length"");

    char* column = (char*)calloc(MAX_TEXT_SIZE + 1, sizeof(char));
    if (column == NULL) return 1;

    for (int len = 1; len <= maxKeyLen; len++) {
        double avgIoC = 0.0;
        for (int k = 0; k < len; k++) {
            int colIdx = 0;
            for (size_t i = (size_t)k; i < textLen && colIdx < MAX_TEXT_SIZE; i += (size_t)len) {
                column[colIdx++] = text[i];
            }
            column[colIdx] = '\0';
            avgIoC += calculateIoC(column);
        }
        avgIoC /= len;

        if (avgIoC > TARGET_IOC && (TARGET_IOC - avgIoC < TARGET_IOC - bestIoC || bestIoC < TARGET_IOC)) {
            bestIoC = avgIoC;
            bestLen = len;
        }
    }

    free(column);
    printf(COLOR_GREEN ""%s: %d (IoC = %.4f)\n"" COLOR_RESET, (lang == 1) ? ""Предполагаемая длина ключа"" : ""Estimated key length"", bestLen, bestIoC);
    return bestLen;
}
void findTopShifts(const char* columnText, ShiftCandidate* top3) {
    int counts[ALPHABET_SIZE] = { 0 };
    int N = 0;
    for (size_t i = 0; columnText[i] != '\0'; i++) {
        int idx = getIndex(columnText[i]);
        if (idx != -1) { counts[idx]++; N++; }
    }

    for (int i = 0; i < MAX_TOP_SHIFTS; i++) {
        top3[i].shift = 0;
        top3[i].chi_score = INF_SCORE;
    }
    if (N == 0) return;

    for (int shift = 0; shift < ALPHABET_SIZE; shift++) {
        double chi = 0.0;
        for (int i = 0; i < ALPHABET_SIZE; i++) {
            int decryptedIdx = (i - shift + ALPHABET_SIZE) % ALPHABET_SIZE;
            double expected = (mixed_freq[decryptedIdx] / 100.0) * N;
            if (expected > 0.0001) {
                chi += ((counts[i] - expected) * (counts[i] - expected)) / expected;
            }
        }

        if (chi < top3[0].chi_score) {
            top3[2] = top3[1]; top3[1] = top3[0];
            top3[0].shift = shift; top3[0].chi_score = chi;
        }
        else if (chi < top3[1].chi_score) {
            top3[2] = top3[1];
            top3[1].shift = shift; top3[1].chi_score = chi;
        }
        else if (chi < top3[2].chi_score) {
            top3[2].shift = shift; top3[2].chi_score = chi;
        }
    }
}
void generateCombinationsIterative(ShiftCandidate shifts[][MAX_TOP_SHIFTS], int keyLen, KeyOption results[], int* resultCount, int maxCombinations) {
    if (keyLen <= 0 || keyLen > DEFAULT_MAX_KEY_LEN) return;
    int indices[DEFAULT_MAX_KEY_LEN] = { 0 };
    *resultCount = 0;

    while (*resultCount < maxCombinations) {
        char currentKey[MAX_KEY_LEN + 1];
        double currentChi = 0.0;

        for (int i = 0; i < keyLen; i++) {
            int choice = indices[i];
            currentKey[i] = ALPHABET[shifts[i][choice].shift];
            currentChi += shifts[i][choice].chi_score;
        }
        currentKey[keyLen] = '\0';

        results[*resultCount].total_chi = currentChi;
        strcpy_s(results[*resultCount].key, MAX_KEY_LEN + 1, currentKey);
        (*resultCount)++;

        int next = keyLen - 1;
        while (next >= 0) {
            indices[next]++;
            if (indices[next] < MAX_TOP_SHIFTS) break;
            indices[next] = 0;
            next--;
        }
        if (next < 0) break;
    }
}
void hackVigenere(const char* cipherText, int lang) {
    size_t textLen = strlen(cipherText);
    if (textLen == 0) return;

    int keyLength = 1;
    printf(COLOR_YELLOW ""%s\n"" COLOR_RESET, (lang == 1) ? ""1. Автоматический подбор\n2. Ручной ввод"" : ""1. Automatic\n2. Manual"");

    int hackMode = 1;
    if (scanf_s(""%d"", &hackMode) != 1) hackMode = 1;
    while (getchar() != '\n');

    if (hackMode == 2) {
        printf(""%s "", (lang == 1) ? ""Длина ключа:"" : ""Key length:"");
        if (scanf_s(""%d"", &keyLength) != 1 || keyLength <= 0 || keyLength > DEFAULT_MAX_KEY_LEN) return;
        while (getchar() != '\n');
    }
    else {
        keyLength = guessKeyLength(cipherText, lang);
    }

    ShiftCandidate top_shifts[DEFAULT_MAX_KEY_LEN + 1][MAX_TOP_SHIFTS];
    char* column = (char*)calloc(MAX_TEXT_SIZE + 1, sizeof(char));
    if (column == NULL) return;

    for (int k = 0; k < keyLength; k++) {
        int colIdx = 0;
        for (size_t i = (size_t)k; i < textLen && colIdx < MAX_TEXT_SIZE; i += (size_t)keyLength) {
            column[colIdx++] = cipherText[i];
        }
        column[colIdx] = '\0';
        findTopShifts(column, top_shifts[k]);
    }
    free(column);

    char* decrypted = (char*)calloc(MAX_TEXT_SIZE, sizeof(char));
    if (decrypted == NULL) return;

    if (keyLength <= 7) {
        int maxCombinations = 1;
        for (int i = 0; i < keyLength; i++) maxCombinations *= MAX_TOP_SHIFTS;

        KeyOption* results = (KeyOption*)calloc(maxCombinations, sizeof(KeyOption));
        if (results != NULL) {
            int resultCount = 0;
            generateCombinationsIterative(top_shifts, keyLength, results, &resultCount, maxCombinations);

            for (int i = 1; i < resultCount; i++) {
                KeyOption keyElement = results[i];
                int j = i - 1;
                while (j >= 0 && results[j].total_chi > keyElement.total_chi) {
                    results[j + 1] = results[j];
                    j--;
                }
                results[j + 1] = keyElement;
            }

            int topN = (resultCount < MAX_TOP_SHIFTS) ? resultCount : MAX_TOP_SHIFTS;
            for (int i = 0; i < topN; i++) {
                printf(COLOR_GREEN ""\n[%d] %s: "" COLOR_BOLD ""%s"" COLOR_GREEN "" (Chi: %.2f)"" COLOR_RESET, i + 1, (lang == 1) ? ""Ключ"" : ""Key"", results[i].key, results[i].total_chi);
                strcpy_s(decrypted, MAX_TEXT_SIZE, cipherText);
                decryptVigenere(decrypted, results[i].key);
                printf(COLOR_CYAN ""\n--- %s ---\n%s\n"" COLOR_RESET, (lang == 1) ? ""Текст"" : ""Text"", decrypted);
            }
            free(results);
        }
    }
    else {
        char bestKey[MAX_KEY_LEN + 1];
        for (int k = 0; k < keyLength; k++) bestKey[k] = ALPHABET[top_shifts[k][0].shift];
        bestKey[keyLength] = '\0';

        printf(COLOR_GREEN ""\n%s: "" COLOR_BOLD ""%s\n"" COLOR_RESET, (lang == 1) ? ""Лучший ключ"" : ""Best key"", bestKey);
        strcpy_s(decrypted, MAX_TEXT_SIZE, cipherText);
        decryptVigenere(decrypted, bestKey);
        printf(COLOR_CYAN ""%s\n%s\n"" COLOR_RESET, (lang == 1) ? ""--- Результат ---"" : ""--- Result ---"", decrypted);
    }
    free(decrypted);
}
";

        var regex = new Regex(
            @"\b((?:const\s+)?(?:unsigned\s+|signed\s+|struct\s+|class\s+)?(?:[a-zA-Z_][a-zA-Z0-9_:]*)\s*[\*\&]*)\s+([a-zA-Z_][a-zA-Z0-9_:]*)\s*\([^)]*\)\s*\{",
            RegexOptions.Compiled);

        var matches = regex.Matches(code);
        Console.WriteLine($"Matches found: {matches.Count}");
        foreach (Match match in matches)
        {
            Console.WriteLine($"Match: {match.Value}");
            Console.WriteLine($"Group 1 (Return Type): '{match.Groups[1].Value}'");
            Console.WriteLine($"Group 2 (Func Name): '{match.Groups[2].Value}'");
            Console.WriteLine("--------------------");
        }
    }
}
