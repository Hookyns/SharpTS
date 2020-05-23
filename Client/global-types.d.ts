interface Window {
	cefQuery(options: { [key: string]: any }): string;
	cefQueryCancel(requestId: string): void;
}