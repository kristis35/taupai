/** Frontend'o konfigūracija iš aplinkos kintamųjų (.env.local). */
export const env = {
  apiUrl: (process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5109").replace(/\/+$/, ""),
};
