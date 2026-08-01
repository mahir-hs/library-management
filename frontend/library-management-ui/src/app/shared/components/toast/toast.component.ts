import { Component, Output, EventEmitter, Input } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastMessage {
  id: string;
  type: ToastType;
  message: string;
}

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [],
  template: `
    <div class="toast-container" aria-live="polite">
      @for (toast of toasts; track toast.id) {
        <div class="toast toast-{{ toast.type }}" role="alert">
          <span class="toast-icon">{{ getIcon(toast.type) }}</span>
          <span class="toast-message">{{ toast.message }}</span>
          <button
            class="toast-close"
            (click)="dismiss.emit(toast.id)"
            aria-label="Close notification"
          >
            &times;
          </button>
        </div>
      }
    </div>
  `,
  styleUrl: './toast.component.scss',
})
export class ToastComponent {
  @Input() toasts: ToastMessage[] = [];
  @Output() dismiss = new EventEmitter<string>();

  private static nextId = 0;

  static create(
    toasts: ToastMessage[],
    type: ToastType,
    message: string,
  ): ToastMessage[] {
    const toast: ToastMessage = {
      id: `toast-${++ToastComponent.nextId}`,
      type,
      message,
    };
    return [...toasts, toast];
  }

  getIcon(type: ToastType): string {
    const icons: Record<ToastType, string> = {
      success: '✅',
      error: '❌',
      warning: '⚠️',
      info: 'ℹ️',
    };
    return icons[type];
  }
}
