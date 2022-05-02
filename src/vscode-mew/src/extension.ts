/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
// tslint:disable
'use strict';

// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import { workspace, Disposable, ExtensionContext } from 'vscode';
import * as lsp from "vscode-languageclient/node";
import { Trace } from 'vscode-jsonrpc';
import { AstProvider } from './astView';
import { existsSync } from "fs";
import * as path from "path";

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export async function activate(context: vscode.ExtensionContext) {
    // Start the language server
    let client = startLanguageServer(context);
    if (client !== null) 
    {
        // Wait until the client is ready
        await client.onReady();

        // Register the AST view
        const astProvider = new AstProvider(client);
        vscode.window.registerTreeDataProvider('astView', astProvider);
        vscode.commands.registerCommand('mew.ast.refreshEntry', () => astProvider.refresh());
    }
}

function startLanguageServer(context: vscode.ExtensionContext) : lsp.LanguageClient|null {

    let serverExe = 'dotnet';
    let executable = getLangServerPath(context);
    if (!existsSync(executable)) {
        vscode.window.showErrorMessage(`Mew language server not found: ${executable}`);
        return null;
    }

    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions: lsp.ServerOptions = {
        run: { command: serverExe, args: [executable] },
        debug: { command: serverExe, args: [executable] }
        // debug: { command: serverExe, args: [executable, "--debug"] }
    }; 

    // Options to control the language client
    let clientOptions: lsp.LanguageClientOptions = {
        // Register the server for plain text documents
        documentSelector: [
            {
                pattern: '**/*.mew',
            }
        ],
        synchronize: {
            // Synchronize the setting section 'languageServerExample' to the server
            configurationSection: 'mewLanguageServer',
            fileEvents: workspace.createFileSystemWatcher('**/*.mew')
        },
    };

    // Create the language client and start the client.
    const client = new lsp.LanguageClient('mewLanguageServer', 'Mew Language Server', serverOptions, clientOptions);
    client.trace = Trace.Verbose;
    let disposable = client.start();
	vscode.window.showInformationMessage(`Mew language server started from ${executable}`);

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);

    return client;
}

function getLangServerPath(context: vscode.ExtensionContext) : string {
    // Got an environment variable?
    let envPath = process.env.MEW_LANGUAGE_SERVER_PATH;
    if (envPath !== undefined) {
        return envPath;
    }

    // In development mode?
    if (context.extensionMode === vscode.ExtensionMode.Development) {
        return path.resolve(path.join(context.extensionPath, `../Mew.LanguageServer/bin/Debug/net6.0/Mew.LanguageServer.dll`));
    }

    return context.asAbsolutePath("bin/Mew.LanguageServer.dll");
}

// this method is called when your extension is deactivated
export function deactivate() {}
