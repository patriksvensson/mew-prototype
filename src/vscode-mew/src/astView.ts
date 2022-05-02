import * as vscode from 'vscode';
import * as lsp from "vscode-languageclient/node";
import * as path from 'path';
import { toJSONObject } from 'vscode-languageclient/lib/common/configuration';

class AstTree {
	name: string;
	kind: string;
	icon: string;
	span?: TextSpan;
	children: Array<AstTree>;

	constructor(name: string, kind: string, icon: string, children: Array<AstTree>, span?: TextSpan) {
		this.name = name;
		this.kind = kind;
		this.icon = icon;
		this.span = span;
		this.children = children;
	}
}

class TextSpan {
	position: number;
	length: number;
	
	constructor(position: number, length: number) {
		this.position = position;
		this.length = length;
	}
}

export class AstProvider implements vscode.TreeDataProvider<AstNode> {

	private _client: lsp.LanguageClient;
	private _root?: AstNode;
	private _onDidChangeTreeData: vscode.EventEmitter<AstNode | undefined | void> = new vscode.EventEmitter<AstNode | undefined | void>();
	readonly onDidChangeTreeData?: vscode.Event<AstNode | undefined | void> = this._onDidChangeTreeData.event;

	constructor(client: lsp.LanguageClient) {
		this._client = client;
		this._client.onNotification("custom/updated", (ast: AstTree) => {
			this._root = this.buildTree(ast);
			this.refresh();
		});
	}

	refresh(): void {
		this._onDidChangeTreeData.fire();
	}
	
	getTreeItem(element: AstNode): vscode.TreeItem | Thenable<vscode.TreeItem> {
		return element;
	}
	
	getChildren(element?: AstNode): vscode.ProviderResult<AstNode[]> {
		if(element === undefined) {
			if(this._root !== undefined) {
				let deps = new Array<AstNode>();
				deps.push(this._root!);
				return Promise.resolve(deps);
			}

			return Promise.resolve([]);
		}

		return Promise.resolve(element!.children);
	}

	private buildTree(tree: AstTree, parent?: AstNode) : AstNode {
		var collapsed = tree.name === "AST" 
			? vscode.TreeItemCollapsibleState.Expanded
			: tree.children.length > 0 
				? vscode.TreeItemCollapsibleState.Collapsed 
				: vscode.TreeItemCollapsibleState.None;

		var node = new AstNode(tree.name, tree.kind, collapsed, `${tree.icon}.svg`, tree.span);
		if(parent !== undefined) {
			parent.children.push(node);
		}

		tree.children.forEach(child => {
			this.buildTree(child, node);
		});

		return node;
	}
}

export class AstNode extends vscode.TreeItem {
	contextValue?: 'dependency';
	children: Array<AstNode>;

	constructor(
		public readonly label: string,
		public readonly trivia: string,
		public readonly collapsibleState: vscode.TreeItemCollapsibleState,
		public readonly icon: string,
		public readonly span?: TextSpan,
		public readonly command?: vscode.Command,
	) {
		super(label, collapsibleState);

		this.children = new Array<AstNode>();
		this.tooltip = this.label;
		this.description = this.trivia;

		if(span !== undefined) {
			this.tooltip = `Span: ${span?.position}:${span?.length}`;
		}

		this.iconPath = {
			light: path.join(__filename, '..', '..', 'resources', 'light', icon),
			dark: path.join(__filename, '..', '..', 'resources', 'dark', icon)
		};
	}
}