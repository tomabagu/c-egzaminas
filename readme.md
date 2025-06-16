V1.0 Pridėtas Master projektas, kuris laukia agent1 ir agent 2 pranešimo. Pridėtas agent1 ir agent2, kuris siunčia pranešimą Master į NamedPipeServerStream
V1.1 Pridėtas failų nuskaitymas AgentA ir AgentB, žodžių agregavimas ir messageQueue pridėjimas, kodo komentarai
V1.2 Pridėta daugiau duomenų ir txt failų testavimui
V1.3 pridėtas duomenų perdavimas per NamedPipe į master, Master pridėtas gaunamų duomenų apdorojimas į ConcurrentDictionary<string, int> duomenų tipą ir duomenų išvedimas į konsolę
V1.4 pridėti papildomi simboliai žodžių atskyrimui
V1.5 pridėtas rikiavimas prieš atvaizduojant žodžių dažnius Master projekte
V1.6 argumentų perdavimas agentui ir master projektui paleidimo metu. Pakoreguotas rikiavimas
V1.7 pakeisti .txt failų pavadinimai, sukauptą žodžių dažnių statiską išvedus į konsolę, išsaugoma į failą "word_counts.txt"