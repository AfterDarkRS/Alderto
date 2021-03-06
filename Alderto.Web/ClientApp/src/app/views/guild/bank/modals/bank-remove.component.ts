import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AldertoWebBankApi, GuildService } from 'src/app/services';
import { IGuildBank } from 'src/app/models';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';


@Component({
    templateUrl: 'bank-remove.component.html'
})
export class BankRemoveComponent implements OnInit, OnDestroy {
    // Input
    public bank: IGuildBank;

    public onBankDeleted: Subject<void>;

    constructor(
        private readonly bankApi: AldertoWebBankApi,
        private readonly guild: GuildService,
        private readonly toastr: ToastrService,
        public readonly modal: BsModalRef) {
    }

    public ngOnInit(): void {
        this.onBankDeleted = new Subject();
    }

    public ngOnDestroy(): void {
        this.onBankDeleted.complete();
    }

    public onDeleteConfirmed() {
        this.bankApi.removeBank(this.guild.currentGuildId, this.bank.id).subscribe(() => {
            this.onBankDeleted.next();
            this.toastr.success(`Successfully removed bank <b>${this.bank.name}</b>`, null, { enableHtml: true });
        },
            (err: HttpErrorResponse) => {
                this.toastr.error(err.error.message, 'Could not remove the bank');
            },
            () => {
                this.modal.hide();
            });
    }
}
