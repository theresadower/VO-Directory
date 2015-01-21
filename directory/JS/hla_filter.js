// Get and apply an XSLT filter to the rows of a table.
// The filter values are stored in the XSLT Processors parameters
// The filter text is of the form:
//   |id1:test1|id2:test2|...|
// where idN is the column number (1-indexed) and testN is the filter.
// The leading pipe allows easy identification of the |id: syntax
// for all elements including the first.

// The types are stored a sequence of 'true/false' values separated
// by commas.  True indicates a character column.
// 
// xslProc is an object with getParameter/setParameter methods that
// stores filtering parameters that need to be passed for subsequent
// XSLT processing (used to create the filter box form).
//
// TAM 2007-9-11

function XSLTFilter(baseDocument, xslProc) {

    var me = this;

    this.setBaseDocument = function(baseDocument) {
        this.baseDocument = baseDocument;
        this.currentDoc = baseDocument;
        // constraints gives parameters that were used to create currentDoc from baseDocument
        this.constraints = {'userIDs': [],
                            'userConstraints': [],
                            'userTypes': [],
                            'filterText': "",
                            'selectedRows': ""};
    };

    this.setBaseDocument(baseDocument);

    this.invalidateFilterDocument = function() {
        // call when base document has been modified
		// forces filtering to be redone but retains current filtering parameters
        this.currentDoc = null;
    };

    this.getDocument = function() {
        this.filterByColumn({
            "filterText": xslProc.getParameter(null,"filterText") || "",
            "filterTypes": xslProc.getParameter(null,"filterTypes")
            }, xslProc.getParameter(null,"selectedRows"));
        return this.currentDoc;
    };

    this.filterByColumn = function(form, selectedRows) {
        // filter columns using current form values
        // returns true if results have changed

        var ff = getConstraints(form, selectedRows);
        if (ff.userIDs.length == 0 && ! ff.selectedRows) {
            // No filtering, so just use the original data.
            xslProc.setParameter(null, "filterText", "");
            xslProc.setParameter(null, "filterTypes", "");
            xslProc.setParameter(null, "selectedRows", "");
            this.currentDoc = this.baseDocument;
            if (this.constraints.filterText || this.constraints.selectedRows) {
                addSelectedInfo("");
                this.constraints = ff;
                return true;
            } else {
                // no change in filter settings
                return false;
            }

        } else {

            xslProc.setParameter(null, "filterText", ff.filterText);
            xslProc.setParameter(null, "filterTypes", ff.userTypes.join(","));
            xslProc.setParameter(null, "selectedRows", ff.selectedRows);
            if (ff.filterText == this.constraints.filterText &&
              ff.selectedRows == this.constraints.selectedRows &&
              this.currentDoc) {
                // no change in filter settings
                return false;
            }

            // don't do processing if baseDocument is null
            var newDoc;
            if (this.baseDocument) {
                var xsltString;
                try {
                    xsltString = xslt(ff.userIDs, ff.userConstraints, ff.userTypes);

                    // Get an XSL processor
                    var xsltp = new XSLTProcessor();
                    var parser = new DOMParser();
                    var xsltDom = parser.parseFromString(xsltString, "text/xml");
                    xsltp.importStylesheet(xsltDom);

                    // hack: add the lookup table for selected fields right into the VOTable
                    addSelectedInfo(ff.selectedRows);

                    // This does the transformation
                    newDoc = xsltp.transformToDocument(this.baseDocument);
                    if (! newDoc.firstChild) {
                        alert("bug: null document");
                        return false;
                    }
                    this.constraints = ff;
                } catch (e) {
                    alert("Error in filtering.  Invalid syntax on field criteria?\n\n"+
                      "For numeric columns use >,>=,=,<,<= or range:\n"+
                      "   >30  or  30..50\n"+
                      "The = operator is optional, so =30 and 30 are equivalent.\n"+
                      "Character fields support only exact matches and matches with "+
                      "wildcards (*).\n"+
                      "   Zwicky    or    3C*273\n"+
                      "If no wildcards are specified the string must match completely. "+
                      "Append a '*' to match select all rows matching at "+
                      "the beginning (i.e., '3C*' matches '3C273').\n"+
                      "Matches are case-insensitive (i.e., '3c273' matches '3C273').\n\n"+
                      "'!' at the beginning selects all rows that do not match.\n\n"+
                       e);
                    return false;
                }
            }
            this.currentDoc = newDoc;
        }
        return true;
    };

    this.clear = function(form) {
        // clear form entries and XSL saved parameters and return true if the
        // filtering parameters changed
        var changed = false;
        if (form) {
            for (var j=0; j<form.elements.length; j++) {
                var el = form.elements[j];
                if (el.tagName == "INPUT" && el.type == "text" && el.value != "") {
                    el.value = null;
                    changed = true;
                }
            }
        }
        this.clearXSL();
        return changed;
    };

    this.clearXSL = function(clearSelected) {
        xslProc.setParameter(null, "filterText", "");
        xslProc.setParameter(null, "filterTypes", "");
        if (clearSelected) xslProc.setParameter(null, "selectedRows", "");
    };

    this.setParameters = function(params) {
        // pass in a dictionary {"filterText": value, ...}
        for (var v in params) {
            xslProc.setParameter(null, v, params[v]);
        }
    };

    // local functions and variables

    function getConstraints(form, selectedRows) {
        // Extract filtering parameters from form
        // form can be either an HTML DOM <form> element or a dictionary
        // containing the filterText and filterTypes parameters
        var userIDs = [];
        var userTypes = [];
        var userConstraints = [];
        selectedRows = selectedRows || "";
        if (! form) {
            // use current constraints if the form is not available
            return me.constraints;
        } else if (form.elements) {
            for (var j=0; j<form.elements.length; j++) {
                var el = form.elements[j];
                if (el.tagName == "INPUT" && el.type == "text") {
                    var constraint = el.value;
                    if (constraint) {
                        // field number is in trailing digits
                        var i = parseInt(el.name.replace(/.*[^0-9]/,""),10);
                        userIDs.push(i);
                        userConstraints.push(constraint);
                        var v = form.elements[el.name+"_type"].value;
                        v = trim(v.toLowerCase());
                        userTypes.push(v);
                    }
                }
            }
            if (userIDs.length > 0) {
                var filterText = new Array(userIDs.length);
                for (i=0; i<userIDs.length; i++) {
                    filterText[i] = userIDs[i] + ":" + userConstraints[i];
                }
                filterText = "|"+filterText.join("|")+"|";
            } else {
                filterText = "";
            }
        } else {
            // Dictionary parameter
            filterText = form.filterText;
            if (filterText) {
                var fields = filterText.substring(1,filterText.length-1).split('\|');
                var types = form.filterTypes.split(',');
                for (i=0; i<fields.length; i += 1) {
                    var index = fields[i].indexOf(':');
                    if (index > 0) {
                        userIDs.push(parseInt(fields[i].substring(0,index),10));
                        userConstraints.push(fields[i].substring(index+1));
                        userTypes.push(types[i]);
                    }
                }
            }
        }
        var constraints = {'userIDs': userIDs,
                        'userConstraints': userConstraints,
                        'userTypes': userTypes,
                        'filterText': filterText,
                        'selectedRows': selectedRows};
        return constraints;
    }

    function addSelectedInfo(selectedRows) {
        // hack: add the lookup table for selected fields right into the
        // me.baseDocument VOTable
        var root;
        for (var i=0; i < me.baseDocument.childNodes.length; i++) {
            if (me.baseDocument.childNodes[i].tagName == "VOTABLE") {
                root = me.baseDocument.childNodes[i];
                break;
            }
        }
        if (! root) {
            alert("Root is null");
            return;
        }

        var selectedNode;
        for (i=0; i < root.childNodes.length; i++) {
            if (root.childNodes[i].tagName == "selected") {
                selectedNode = root.childNodes[i];
                while(selectedNode.firstChild) {
                    selectedNode.removeChild(selectedNode.firstChild);
                }
                break;
            }
        }

        var namespace = root.namespaceURI || "";

        if (! selectedNode) {
            // add the <selected> node
            if (me.baseDocument.createElementNS) {
                selectedNode = me.baseDocument.createElementNS(namespace, "selected");
            } else {
                // IE
                selectedNode = me.baseDocument.createNode(1,"selected",namespace);
            }
            root.insertBefore(selectedNode, root.firstChild);
        }

        var srows = selectedRows.split(',');
        for (i=0; i<srows.length; i++) {
            if (srows[i]) {
                if (me.baseDocument.createElementNS) {
                    var el = me.baseDocument.createElementNS(namespace, "name");
                    el.textContent = srows[i];
                } else {
                    // IE
                    el = me.baseDocument.createNode(1,"name",namespace);
                    var tnode = me.baseDocument.createTextNode(srows[i]);
                    el.appendChild(tnode);
                }
                selectedNode.appendChild(el);
            }
        }
    }
}
