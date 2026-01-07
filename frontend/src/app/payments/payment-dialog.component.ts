import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@angular/cdk/dialog';

@Component({
  selector: 'app-payment-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="payment-dialog">
      <h3>Make Payment</h3>
      <form #payForm="ngForm" (ngSubmit)="submit()">
        <label>
          Amount:
          <input name="amount" [(ngModel)]="amount" #amt="ngModel" type="number" step="0.01" required />
          <div class="field-error" *ngIf="amt.invalid && amt.touched">Amount is required.</div>
        </label>
        <div class="actions">
          <button type="button" (click)="cancel()">Cancel</button>
          <button type="submit">Pay</button>
        </div>
      </form>
    </div>
  `,
  styles: [
    `.payment-dialog { background: #ffffff; padding: 1rem; border-radius: 6px; box-shadow: 0 6px 18px rgba(0,0,0,0.12); }
     .payment-dialog h3 { margin: 0 0 0.5rem 0; }
     .payment-dialog label { display: block; margin: 0.5rem 0; }
     .payment-dialog input { width: 100%; padding: 0.4rem; box-sizing: border-box; }
     .actions { margin-top: 1rem; display:flex; gap:8px; justify-content:flex-end }
     button { padding: 0.4rem 0.8rem; }
    `,
  ]
})
export class PaymentDialogComponent {
  dialogRef = inject(DialogRef<number | undefined>);
  amount: number | null = null;

  cancel() {
    this.dialogRef.close(undefined);
  }

  submit() {
    // return the entered amount (undefined if null)
    this.dialogRef.close(this.amount == null ? undefined : this.amount);
  }
}
