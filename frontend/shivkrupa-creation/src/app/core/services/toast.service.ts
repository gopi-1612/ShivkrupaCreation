import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: number;
  type: ToastType;
  message: string;
}

/**
 * Lightweight, dependency-free toast/notification queue. The shared
 * ToastContainerComponent renders whatever is in `toasts()`; any service or
 * component can push a message here without needing a template reference.
 */
@Injectable({ providedIn: 'root' })
export class ToastService {
  private nextId = 1;
  readonly toasts = signal<Toast[]>([]);

  success(message: string) {
    this.push('success', message);
  }

  error(message: string) {
    this.push('error', message, 6000);
  }

  info(message: string) {
    this.push('info', message);
  }

  warning(message: string) {
    this.push('warning', message);
  }

  dismiss(id: number) {
    this.toasts.update((list) => list.filter((t) => t.id !== id));
  }

  private push(type: ToastType, message: string, durationMs = 4000) {
    const id = this.nextId++;
    this.toasts.update((list) => [...list, { id, type, message }]);
    setTimeout(() => this.dismiss(id), durationMs);
  }
}
