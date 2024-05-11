{{- define "helpers.list-api-deployment-env-variables" }}
{{- range $key, $val := .Values.api.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.api.env.secret }}
- name: "EDINBURGH__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-project-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-bff-deployment-env-variables" }}
{{- range $key, $val := .Values.bffweb.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.bffweb.env.secret }}
- name: "EDINBURGH__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-project-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-web-deployment-env-variables" }}
{{- range $key, $val := .Values.web.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.web.env.secret }}
- name: {{ $key }}
  valueFrom:
    secretKeyRef:
      name: 'zeno-project-secret'
      key: {{ $key }}
{{- end}}
{{- end }}