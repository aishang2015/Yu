import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { LocalStorageService } from '../services/local-storage.service';

@Directive({ selector: '[canOperate]' })
export class CanOperateDirective {

    private hasView = false;

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef,
        private _localStorageService: LocalStorageService) { }

    @Input() set canOperate(identification: string) {

        // 判断是否包含此元素的操作权
        var identifycations = this._localStorageService.getIdentifycations();
        let hasRight = identifycations.findIndex(i => i == identification) > -1;

        if (hasRight && !this.hasView) {
            this.viewContainer.createEmbeddedView(this.templateRef);
            this.hasView = true;
        } else if (!hasRight && this.hasView) {
            this.viewContainer.clear();
            this.hasView = false;
        }
    }
}