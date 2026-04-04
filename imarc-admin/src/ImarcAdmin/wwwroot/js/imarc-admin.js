window.imarcAdmin = {
  copyFromInput: async function (inputId) {
    const input = document.getElementById(inputId);
    if (!input) {
      return false;
    }

    try {
      if (navigator.clipboard && window.isSecureContext) {
        await navigator.clipboard.writeText(input.value);
        return true;
      }
    } catch (error) {
      console.warn("Clipboard API failed, falling back to manual selection.", error);
    }

    input.focus();
    input.select();
    input.setSelectionRange(0, input.value.length);

    try {
      return document.execCommand("copy");
    } catch (error) {
      console.warn("document.execCommand copy failed.", error);
      return false;
    }
  },

  insertAtCursor: function (textareaId, snippet) {
    const textarea = document.getElementById(textareaId);
    if (!textarea) {
      return;
    }

    const start = textarea.selectionStart || 0;
    const end = textarea.selectionEnd || 0;
    const current = textarea.value || "";
    const nextValue = current.slice(0, start) + snippet + current.slice(end);
    textarea.value = nextValue;
    const cursor = start + snippet.length;
    textarea.selectionStart = cursor;
    textarea.selectionEnd = cursor;
    textarea.dispatchEvent(new Event("input", { bubbles: true }));
    textarea.focus();
  }
};

