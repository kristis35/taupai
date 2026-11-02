import { Button } from "@/components/ui/button";

export default function Home() {
  return (
    <main className="flex flex-1 flex-col items-center justify-center gap-6 px-4 py-16 text-center">
      <h1 className="text-4xl font-semibold tracking-tight">SąskaitųAgentas.lt</h1>
      <p className="max-w-md text-muted-foreground">
        Asmeninis agentas, kuris nuolat mažina jūsų namų ūkio sąskaitas.
      </p>
      <Button size="lg">Sutvarkyk už mane</Button>
    </main>
  );
}
